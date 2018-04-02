namespace AP.Business.Registration.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Infrastructure.Messaging.Handling;
    using AP.EntityModel.ReadModel;
    using AP.Business.Model.Registration.Events;
    using AP.Business.Model.Registration;
    using AP.Repository.Context;
    using Microsoft.EntityFrameworkCore;
    using EntityFramework.DbContextScope.Interfaces;
    using AP.Core.Database;
    using Microsoft.Extensions.Logging;

    public class DraftOrderViewModelGenerator : AmbientContext<WorkshopRegistrationDbContext>,
        IEventHandler<OrderPlaced>, IEventHandler<OrderUpdated>,
        IEventHandler<OrderPartiallyReserved>, IEventHandler<OrderReservationCompleted>,
        IEventHandler<OrderRegistrantAssigned>,
        IEventHandler<OrderConfirmed>
    {
        private readonly IDbContextScopeFactory factory;
        private readonly ILogger<DraftOrderViewModelGenerator> logger;

        public DraftOrderViewModelGenerator(IDbContextScopeFactory factory, 
            ILogger<DraftOrderViewModelGenerator> logger,
            IAmbientDbContextLocator locator) : base (locator)
        {
            this.factory = factory;
            this.logger = logger;
        }

        public void Handle(OrderPlaced @event)
        {
            using (var context = factory.Create())
            {
                var dto = new DraftOrder(@event.SourceId, @event.WorkshopId, DraftOrder.States.PendingReservation, @event.Version)
                {
                    AccessCode = @event.AccessCode,
                };

                dto.Lines.AddRange(@event.Anchors.Select(anchor => new DraftOrderItem(anchor.AnchorType, anchor.Quantity)));
                DbContext.Save(dto);

                context.SaveChanges();
            }
        }

        public void Handle(OrderRegistrantAssigned @event)
        {
            using (var context = factory.Create())
            {
                var dto = DbContext.Find<DraftOrder>(@event.SourceId);
                if (WasNotAlreadyHandled(dto, @event.Version))
                {
                    dto.AttendeeID = @event.AttendeeID;
                    dto.OrderVersion = @event.Version;
                    DbContext.Save(dto);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(OrderUpdated @event)
        {
            using (var context = factory.Create())
            {
                var dto = DbContext.Set<DraftOrder>().Include(o => o.Lines).First(o => o.OrderID == @event.SourceId);
                if (WasNotAlreadyHandled(dto, @event.Version))
                {
                    var linesSet = DbContext.Set<DraftOrderItem>();
                    foreach (var line in dto.Lines.ToArray())
                    {
                        linesSet.Remove(line);
                    }

                    dto.Lines.AddRange(@event.Anchors.Select(anchor => new DraftOrderItem(anchor.AnchorType, anchor.Quantity)));

                    dto.State = DraftOrder.States.PendingReservation;
                    dto.OrderVersion = @event.Version;
                    //DbContext.Save(dto);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(OrderPartiallyReserved @event)
        {
            this.UpdateReserved(@event.SourceId, @event.ReservationExpiration, DraftOrder.States.PartiallyReserved, @event.Version, @event.Anchors);
        }

        public void Handle(OrderReservationCompleted @event)
        {
            this.UpdateReserved(@event.SourceId, @event.ReservationExpiration, DraftOrder.States.ReservationCompleted, @event.Version, @event.Anchors);
        }

        public void Handle(OrderConfirmed @event)
        {
            using (var context = factory.Create())
            {
                var dto = DbContext.Find<DraftOrder>(@event.SourceId);
                if (WasNotAlreadyHandled(dto, @event.Version))
                {
                    dto.State = DraftOrder.States.Confirmed;
                    dto.OrderVersion = @event.Version;
                    DbContext.Save(dto);
                    context.SaveChanges();
                }
            }
        }

        private void UpdateReserved(Guid orderId, DateTime reservationExpiration, DraftOrder.States state, int orderVersion, IEnumerable<AnchorQuantity> anchors)
        {
            using (var context = this.factory.Create())
            {
                var dto = DbContext.Set<DraftOrder>().Include(x => x.Lines).First(x => x.OrderID == orderId);
                if (WasNotAlreadyHandled(dto, orderVersion))
                {
                    foreach (var anchor in anchors)
                    {
                        var item = dto.Lines.Single(x => x.AnchorType == anchor.AnchorType);
                        item.ReservedAnchors = anchor.Quantity;
                    }

                    dto.State = state;
                    dto.ReservationExpirationDate = reservationExpiration;

                    dto.OrderVersion = orderVersion;
                    DbContext.Save(dto);
                    context.SaveChanges();
                }
            }
        }

        private bool WasNotAlreadyHandled(DraftOrder draftOrder, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > draftOrder.OrderVersion)
            {
                return true;
            }
            else if (eventVersion == draftOrder.OrderVersion)
            {
                logger.LogWarning(
                    "Ignoring duplicate draft order update message with version {1} for order id {0}",
                    draftOrder.OrderID,
                    eventVersion);
                return false;
            }
            else
            {
                logger.LogWarning(
                    @"An older order update message was received with with version {1} for order id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order.",
                    draftOrder.OrderID,
                    eventVersion,
                    draftOrder.OrderVersion);
                return false;
            }
        }
    }
}
