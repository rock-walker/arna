namespace AP.Business.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using AP.Infrastructure.EventSourcing;
    using AP.Business.Model.Registration;
    using AP.Business.Model.Registration.Events;
    using AP.Infrastructure.Utils;

    /// <summary>
    /// Represents an order placed by a user.
    /// </summary>
    /// <remarks>
    /// <para>For more information on the domain, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.</para>
    /// <para>The order is not final at time of creation, and goes through several steps until it is finally completed.</para>
    /// <para>This entity does not implement the <see cref="IMementoOriginator"/> interface because we do not expect each instance
    /// to have a long event stream, nor each instance will be accessed very frequently and from the same process to benefit from in-memory caching.</para>
    /// </remarks>
    public class Order : EventSourced
    {
        /// <summary>
        /// Suggest a anchors reservation expiration time in case the Order is not completed before this time ellapses.
        /// </summary>
        private static readonly TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(15);

        private List<AnchorQuantity> anchors;
        private bool isConfirmed;
        private Guid workshopId;
        /*
        /// <summary>
        /// Mapping old version of the <see cref="OrderPaymentConfirmed"/> event to the new version (<see cref="OrderConfirmed"/>).
        /// Currently it is being done explicitly by the consumer, but this one in particular could be done
        /// at the deserialization level, as it is just a rename, not a functionality change.
        /// <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258556">Journey chapter 6</see> for more information.
        /// </summary>
        
        static Order()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<OrderPaymentConfirmed, OrderConfirmed>());
        }
        */
        protected Order(Guid id)
            : base(id)
        {
            base.Handles<OrderPlaced>(this.OnOrderPlaced);
            base.Handles<OrderUpdated>(this.OnOrderUpdated);
            base.Handles<OrderPartiallyReserved>(this.OnOrderPartiallyReserved);
            base.Handles<OrderReservationCompleted>(this.OnOrderReservationCompleted);
            base.Handles<OrderExpired>(this.OnOrderExpired);
            base.Handles<OrderPaymentConfirmed>(e => this.OnOrderConfirmed(Mapper.Map<OrderConfirmed>(e)));
            base.Handles<OrderConfirmed>(this.OnOrderConfirmed);
            base.Handles<OrderRegistrantAssigned>(this.OnOrderRegistrantAssigned);
            base.Handles<OrderTotalsCalculated>(this.OnOrderTotalsCalculated);
        }

        public Order(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            this.LoadFrom(history);
        }

        /// <summary>
        /// Creates a new order with the specified items and id.
        /// </summary>
        /// <remarks>
        /// The total is calculated at creation time. This was a change done in the V3 version of the system to optimize 
        /// the UI workflow. 
        /// See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more information on 
        /// the optimization and migration to V3.
        /// </remarks>
        /// <param name="id">The identifier.</param>
        /// <param name="workshopId">The conference that the order applies to.</param>
        /// <param name="items">The desired anchors to register to.</param>
        /// <param name="pricingService">Service that calculates the pricing.</param>
        public Order(Guid id, Guid workshopId, IEnumerable<OrderItem> items, IPricingService pricingService)
            : this(id)
        {
            var all = ConvertItems(items);
            var totals = pricingService.CalculateTotal(workshopId, all.AsReadOnly());

            this.Update(new OrderPlaced
            {
                WorkshopId = workshopId,
                Anchors = all,
                ReservationAutoExpiration = DateTime.UtcNow.Add(ReservationAutoExpiration),
                AccessCode = HandleGenerator.Generate(6)
            });
            //TODO: IsFreeOfCharge back to normal calculations: totals.Total == 0m
            this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = true });
        }

        /// <summary>
        /// Updates the order with the specified items.
        /// </summary>
        /// <remarks>
        /// The total is now calculated at this time. This was a change done in the V3 version of the system to optimize 
        /// the UI workflow. 
        /// See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more information on 
        /// the optimization and migration to V3.
        /// </remarks>
        /// <param name="items">The desired anchors to register to.</param>
        /// <param name="pricingService">Service that calculates the pricing.</param>
        public void UpdateAnchors(IEnumerable<OrderItem> items, IPricingService pricingService)
        {
            var all = ConvertItems(items);
            var totals = pricingService.CalculateTotal(this.workshopId, all.AsReadOnly());

            this.Update(new OrderUpdated { Anchors = all });
            //TODO: IsFreeOfCharge back to normal calculations: totals.Total == 0m
            this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = true });
        }

        public void MarkAsReserved(IPricingService pricingService, DateTime expirationDate, IEnumerable<AnchorQuantity> reservedAnchors)
        {
            if (this.isConfirmed)
                throw new InvalidOperationException("Cannot modify a confirmed order.");

            var reserved = reservedAnchors.ToList();

            // Is there an order item which didn't get an exact reservation?
            if (this.anchors.Any(item => item.Quantity != 0 && !reserved.Any(anchor => anchor.AnchorType == item.AnchorType && anchor.Quantity == item.Quantity)))
            {
                var totals = pricingService.CalculateTotal(this.workshopId, reserved.AsReadOnly());

                this.Update(new OrderPartiallyReserved { ReservationExpiration = expirationDate, Anchors = reserved.ToArray() });
                //TODO: IsFreeOfCharge back to normal calculations: totals.Total == 0m
                this.Update(new OrderTotalsCalculated { Total = totals.Total, Lines = totals.Lines != null ? totals.Lines.ToArray() : null, IsFreeOfCharge = true });
            }
            else
            {
                this.Update(new OrderReservationCompleted { ReservationExpiration = expirationDate, Anchors = reserved.ToArray() });
            }
        }

        public void Expire()
        {
            if (this.isConfirmed)
                throw new InvalidOperationException("Cannot expire a confirmed order.");

            this.Update(new OrderExpired());
        }

        public void Confirm()
        {
            this.Update(new OrderConfirmed());
        }

        public void AssignRegistrant(string lastName, string email)
        {
            this.Update(new OrderRegistrantAssigned
            {
                LastName = lastName,
                Email = email,
            });
        }

        public AnchorAssignments CreateAnchorAssignments()
        {
            if (!this.isConfirmed)
                throw new InvalidOperationException("Cannot create anchor assignments for an order that isn't confirmed yet.");

            return new AnchorAssignments(this.Id, this.anchors.AsReadOnly());
        }

        private static List<AnchorQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            return items.Select(x => new AnchorQuantity(x.AnchorType, x.Quantity)).ToList();
        }

        private void OnOrderPlaced(OrderPlaced e)
        {
            this.workshopId = e.WorkshopId;
            this.anchors = e.Anchors.ToList();
        }

        private void OnOrderUpdated(OrderUpdated e)
        {
            this.anchors = e.Anchors.ToList();
        }

        private void OnOrderPartiallyReserved(OrderPartiallyReserved e)
        {
            this.anchors = e.Anchors.ToList();
        }

        private void OnOrderReservationCompleted(OrderReservationCompleted e)
        {
            this.anchors = e.Anchors.ToList();
        }

        private void OnOrderExpired(OrderExpired e)
        {
        }

        private void OnOrderConfirmed(OrderConfirmed e)
        {
            this.isConfirmed = true;
        }

        private void OnOrderRegistrantAssigned(OrderRegistrantAssigned e)
        {
        }

        private void OnOrderTotalsCalculated(OrderTotalsCalculated e)
        {
        }
    }
}
