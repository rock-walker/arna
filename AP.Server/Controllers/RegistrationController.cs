namespace AP.Server.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Messaging;
    using Infrastructure.Utils;
    using AP.EntityModel.ReadModel;
    using Microsoft.AspNetCore.Mvc;
    using AP.Business.Registration.Commands;
    using AP.Business.Registration.ReadModel;
    using AP.ViewModel.Booking;
    using AP.Core.DateTime;
    using AP.Core.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using EntityFramework.DbContextScope.Interfaces;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using System.Security.Principal;

    [Authorize(Roles = "Client,Administrator,PowerUser")]
    [Route("api/[controller]/[action]")]
    public class RegistrationController : WorkshopTenantController
    {
        private static readonly TimeSpan DraftOrderWaitTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DraftOrderPollInterval = TimeSpan.FromMilliseconds(750);
        private static readonly TimeSpan PricedOrderWaitTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan PricedOrderPollInterval = TimeSpan.FromMilliseconds(750);
        private IIdentity LoggedInUser => User.Identity;

        private readonly ICommandBus commandBus;
        private readonly IOrderDao orderDao;
        private readonly IDbContextScopeFactory _factory;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(ICommandBus commandBus,
            IOrderDao orderDao, 
            IWorkshopDao workshopDao, 
            IDbContextScopeFactory factory,
            ILogger<RegistrationController> logger)
            : base(workshopDao)
        {
            this.commandBus = commandBus;
            this.orderDao = orderDao;
            _factory = factory;
            _logger = logger;
        }

        [HttpGet]
        [Route("/{workshopCode}/register")]
        public Task<OrderViewModel> StartRegistration(Guid? orderId = null)
        {
            var viewModelTask = Task.Factory.StartNew(() => CreateViewModel());
            if (!orderId.HasValue)
            {
                return viewModelTask
                    .ContinueWith<OrderViewModel>(t =>
                    {
                        var viewModel = t.Result;
                        viewModel.OrderId = GuidUtil.NewSequentialId();
                        return viewModel;
                    });
            }
            else
            {
                return Task.Factory.ContinueWhenAll<OrderViewModel>(
                    new Task[] { viewModelTask, WaitUntilAnchorsAreConfirmed(orderId.Value, 0) },
                    tasks =>
                    {
                        var viewModel = ((Task<OrderViewModel>)tasks[0]).Result;
                        var order = ((Task<DraftOrder>)tasks[1]).Result;

                        if (order == null)
                        {
                            //return View("PricedOrderUnknown");

                            return viewModel;
                        }

                        if (order.State == DraftOrder.States.Confirmed)
                        {
                            return viewModel; //View("ShowCompletedOrder");
                        }

                        if (order.ReservationExpirationDate.HasValue && order.ReservationExpirationDate < DateTime.UtcNow)
                        {
                            return null;//RedirectToAction("ShowExpiredOrder", new { conferenceCode = this.WorkshopAlias.Code, orderId = orderId });
                        }

                        UpdateViewModel(viewModel, order);
                        return viewModel;
                    });
            }
        }

        [HttpPost]
        [Route("/{workshopCode}/register")]
        public object StartRegistration([FromBody]RegisterToWorkshop command, int orderVersion)
        {
            var existingOrder = orderVersion != 0 ? orderDao.FindDraftOrder(command.OrderId) : null;
            var viewModel = this.CreateViewModel();
            if (existingOrder != null)
            {
                UpdateViewModel(viewModel, existingOrder);
            }

            viewModel.OrderId = command.OrderId;

            if (!ModelState.IsValid)
            {
                return viewModel;
            }

            // checks that there are still enough available seats, and the seat type IDs submitted are valid.
            ModelState.Clear();
            bool needsExtraValidation = false;
            foreach (var seat in command.Anchors)
            {
                var modelItem = viewModel.Items.FirstOrDefault(x => x.AnchorType.ID == seat.AnchorType);
                if (modelItem != null)
                {
                    if (seat.Quantity > modelItem.MaxSelectionQuantity)
                    {
                        modelItem.PartiallyFulfilled = needsExtraValidation = true;
                        modelItem.OrderItem.ReservedAnchors = modelItem.MaxSelectionQuantity;
                    }
                }
                else
                {
                    // seat type no longer exists for conference.
                    needsExtraValidation = true;
                }
            }

            if (needsExtraValidation)
            {
                return viewModel;
            }

            command.WorkshopId = WorkshopAlias.Id;
            commandBus.Send(command);

            //return RedirectToAction(
            //    "SpecifyRegistrantAndPaymentDetails",
            return new { workshopCode = WorkshopCode, orderId = command.OrderId, orderVersion = orderVersion };
        }

        [HttpGet]
        [Route("/{workshopCode}/registrant/{orderId:guid}/{orderVersion?}")]
        public Task<object> SpecifyRegistrantAndPaymentDetails(Guid orderId, int orderVersion)
        {
            return WaitUntilOrderIsPriced(orderId, orderVersion)
                .ContinueWith<object>(t =>
                {
                    _logger.LogDebug(string.Format("Returns priced order after {0} polling. Time: {1}", PricedOrderPollInterval, DateTime.UtcNow));

                    var pricedOrder = t.Result;
                    if (pricedOrder == null)
                    {
                        return 0;//View("PricedOrderUnknown");
                    }

                    if (!pricedOrder.ReservationExpirationDate.HasValue)
                    {
                        return 1;// View("ShowCompletedOrder");
                    }

                    if (pricedOrder.ReservationExpirationDate < DateTime.UtcNow)
                    {
                        return 2; // RedirectToAction("ShowExpiredOrder", new { conferenceCode = this.ConferenceAlias.Code, orderId = orderId });
                    }

                    return new RegistrationViewModel
                        {
                            RegistrantDetails = new AssignRegistrantDetails { OrderId = orderId },
                            Order = pricedOrder
                        };
                });
        }

        [HttpPost]
        [Route("/{workshopCode}/registrant")]
        public Task<object> SpecifyRegistrantAndPaymentDetails([FromBody]AssignRegistrantDetails command, string paymentType, int orderVersion)
        {
            var orderId = command.OrderId;

            if (!ModelState.IsValid)
            {
                return SpecifyRegistrantAndPaymentDetails(orderId, orderVersion);
            }

            commandBus.Send(command);

            return StartPayment(orderId, paymentType, orderVersion);
        }


        [HttpPost]
        [Route("/{workshopCode}/pay/{orderId}")]
        public Task<object> StartPayment(Guid orderId, string paymentType, int orderVersion)
        {
            return WaitUntilAnchorsAreConfirmed(orderId, orderVersion)
                .ContinueWith<object>(t =>
                {
                    var order = t.Result;
                    if (order == null)
                    {
                        return 0;// View("ReservationUnknown");
                    }

                    if (order.State == DraftOrder.States.PartiallyReserved)
                    {
                        //TODO: have a clear message in the UI saying there was a problem and he actually didn't get all the seats.
                        // This happened as a result the seats availability being eventually but not fully consistent when
                        // starting the reservation. It is very uncommon to reach this step, but could happen under heavy
                        // load, and when competing for the last remaining seats of the conference.
                        return /*RedirectToAction("StartRegistration", */ new { conferenceCode = this.WorkshopCode, orderId, orderVersion = order.OrderVersion };
                    }

                    if (order.State == DraftOrder.States.Confirmed)
                    {
                        return 2;// View("ShowCompletedOrder");
                    }

                    if (order.ReservationExpirationDate.HasValue && order.ReservationExpirationDate < DateTime.UtcNow)
                    {
                        return /*RedirectToAction("ShowExpiredOrder", */new { conferenceCode = this.WorkshopAlias.Code, orderId = orderId };
                    }

                    var pricedOrder = this.orderDao.FindPricedOrder(orderId);
                    if (pricedOrder.IsFreeOfCharge)
                    {
                        return CompleteRegistrationWithoutPayment(orderId);
                    }

                    throw new InvalidOperationException();
                });
        }

        [HttpGet]
        public ActionResult ShowExpiredOrder(Guid orderId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult ThankYou(Guid orderId)
        {
            var order = this.orderDao.FindDraftOrder(orderId);

            return View(order);
        }
        /*
        private ActionResult CompleteRegistrationWithThirdPartyProcessorPayment(PricedOrder order, int orderVersion)
        {
            var paymentCommand = CreatePaymentCommand(order);

            this.commandBus.Send(paymentCommand);

            var paymentAcceptedUrl = this.Url.Action("ThankYou", new { conferenceCode = this.ConferenceAlias.Code, order.OrderId });
            var paymentRejectedUrl = this.Url.Action("SpecifyRegistrantAndPaymentDetails", new { conferenceCode = this.ConferenceAlias.Code, orderId = order.OrderId, orderVersion });

            return RedirectToAction(
                "ThirdPartyProcessorPayment",
                "Payment",
                new
                {
                    conferenceCode = this.ConferenceAlias.Code,
                    paymentId = paymentCommand.PaymentId,
                    paymentAcceptedUrl,
                    paymentRejectedUrl
                });
        }
        
        private InitiateThirdPartyProcessorPayment CreatePaymentCommand(PricedOrder order)
        {
            // TODO: should add the line items?

            var description = "Registration for " + this.ConferenceAlias.Name;
            var totalAmount = order.Total;

            var paymentCommand =
                new InitiateThirdPartyProcessorPayment
                {
                    PaymentId = GuidUtil.NewSequentialId(),
                    ConferenceId = this.ConferenceAlias.Id,
                    PaymentSourceId = order.OrderId,
                    Description = description,
                    TotalAmount = totalAmount
                };

            return paymentCommand;
        }
        */

        private object CompleteRegistrationWithoutPayment(Guid orderId)
        {
            var confirmationCommand = new ConfirmOrder { OrderId = orderId };

            commandBus.Send(confirmationCommand);

            using (var context = _factory.CreateReadOnly())
            {
                return new { conferenceCode = this.WorkshopAlias.Code, orderId };
            }
        }

        private OrderViewModel CreateViewModel()
        {
            IList<AnchorType> anchorTypes;
            using (var scope = _factory.CreateReadOnly())
            {
                anchorTypes = WorkshopDao.GetPublishedAnchorTypes(WorkshopAlias.Id);
            }

            var viewModel =
                new OrderViewModel
                {
                    WorkshopId = WorkshopAlias.Id,
                    WorkshopCode = WorkshopAlias.Code,
                    WorkshopName = WorkshopAlias.Name,
                    Items =
                        anchorTypes.Select(
                            s =>
                                new OrderItemViewModel
                                {
                                    AnchorType = s,
                                    OrderItem = new DraftOrderItem(s.ID, 0),
                                    AvailableQuantityForOrder = Math.Max(s.AvailableQuantity, 0),
                                    MaxSelectionQuantity = Math.Max(Math.Min(s.AvailableQuantity, 20), 0)
                                }).ToList(),
                };

            return viewModel;
        }

        private static void UpdateViewModel(OrderViewModel viewModel, DraftOrder order)
        {
            viewModel.OrderId = order.OrderID;
            viewModel.OrderVersion = order.OrderVersion;
            viewModel.ReservationExpirationDate = order.ReservationExpirationDate.ToEpochMilliseconds();

            // TODO check DTO matches view model

            foreach (var line in order.Lines)
            {
                var seat = viewModel.Items.First(s => s.AnchorType.ID == line.AnchorType);
                seat.OrderItem = line;
                seat.AvailableQuantityForOrder = seat.AvailableQuantityForOrder + line.ReservedAnchors;
                seat.MaxSelectionQuantity = Math.Min(seat.AvailableQuantityForOrder, 20);
                seat.PartiallyFulfilled = line.RequestedAnchors > line.ReservedAnchors;
            }
        }

        private Task<DraftOrder> WaitUntilAnchorsAreConfirmed(Guid orderId, int lastOrderVersion) 
        {
            return
                TimerTaskFactory.StartNew<DraftOrder>(
                    () => orderDao.FindDraftOrder(orderId),
                    order => order != null && order.State != DraftOrder.States.PendingReservation && order.OrderVersion > lastOrderVersion,
                    DraftOrderPollInterval,
                    DraftOrderWaitTimeout);
        }

        private Task<PricedOrder> WaitUntilOrderIsPriced(Guid orderId, int lastOrderVersion)
        {
            return
                TimerTaskFactory.StartNew<PricedOrder>(
                    () => orderDao.FindPricedOrder(orderId),
                    order => order != null && order.OrderVersion > lastOrderVersion,
                    PricedOrderPollInterval,
                    PricedOrderWaitTimeout);
        }
    }
}
