namespace AP.Registration.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Messaging.Handling;
    using AP.EntityModel.ReadModel;
    using AP.Business.Model.Registration.Events;
    using Microsoft.EntityFrameworkCore;
    using AP.EntityModel.ReadModel.Implementation;
    using Microsoft.Extensions.Caching.Memory;
    using AP.Business.AutoPortal.Events;
    using AP.Business.Model.Registration;
    using AP.Repository.Context;
    using AP.Core.Database;
    using EntityFramework.DbContextScope.Interfaces;

    public class PricedOrderViewModelGenerator : AmbientContext<WorkshopRegistrationDbContext>,
        IEventHandler<OrderPlaced>,
        IEventHandler<OrderTotalsCalculated>,
        IEventHandler<OrderConfirmed>,
        IEventHandler<OrderExpired>,
        IEventHandler<AnchorAssignmentsCreated>,
        IEventHandler<AnchorCreated>,
        IEventHandler<AnchorUpdated>
    {
        private readonly IDbContextScopeFactory contextFactory;
        //private readonly ObjectCache seatDescriptionsCache;
        private readonly IMemoryCache seatDescriptionsCache;

        public PricedOrderViewModelGenerator(IDbContextScopeFactory contextFactory, IAmbientDbContextLocator locator, IMemoryCache cache)
            :base(locator)
        {
            this.contextFactory = contextFactory;
            //this.seatDescriptionsCache = MemoryCache.Default;
            seatDescriptionsCache = cache;
        }

        public void Handle(OrderPlaced @event)
        {
            using (var context = this.contextFactory.Create())
            {
                var dto = new PricedOrder
                {
                    OrderId = @event.SourceId,
                    ReservationExpirationDate = @event.ReservationAutoExpiration,
                    OrderVersion = @event.Version
                };
                DbContext.Set<PricedOrder>().Add(dto);
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    /*Trace.TraceWarning(
                        "Ignoring OrderPlaced message with version {1} for order id {0}. This could be caused because the message was already handled and the PricedOrder entity was already created.",
                        dto.OrderId,
                        @event.Version);*/
                }
            }
        }

        public void Handle(OrderTotalsCalculated @event)
        {
            var seatTypeIds = @event.Lines.OfType<SeatOrderLine>().Select(x => x.SeatType).Distinct().ToArray();
            using (var context = this.contextFactory.Create())
            {
                var dto = DbContext.Query<PricedOrder>().Include(x => x.Lines).First(x => x.OrderId == @event.SourceId);
                if (!WasNotAlreadyHandled(dto, @event.Version))
                {
                    // message already handled, skip.
                    return;
                }

                var linesSet = DbContext.Set<PricedOrderLine>();
                foreach (var line in dto.Lines.ToList())
                {
                    linesSet.Remove(line);
                }

                var seatTypeDescriptions = GetSeatTypeDescriptions(seatTypeIds);

                for (int i = 0; i < @event.Lines.Length; i++)
                {
                    var orderLine = @event.Lines[i];
                    var line = new PricedOrderLine
                    {
                        LineTotal = orderLine.LineTotal,
                        Position = i,
                    };

                    var seatOrderLine = orderLine as SeatOrderLine;
                    if (seatOrderLine != null)
                    {
                        // should we update the view model to avoid losing the SeatTypeId?
                        line.Description = seatTypeDescriptions.Where(x => x.SeatTypeID == seatOrderLine.SeatType).Select(x => x.Name).FirstOrDefault();
                        line.UnitPrice = seatOrderLine.UnitPrice;
                        line.Quantity = seatOrderLine.Quantity;
                    }

                    dto.Lines.Add(line);
                }

                dto.Total = @event.Total;
                dto.IsFreeOfCharge = @event.IsFreeOfCharge;
                dto.OrderVersion = @event.Version;

                context.SaveChanges();
            }
        }

        public void Handle(OrderConfirmed @event)
        {
            using (var context = this.contextFactory.Create())
            {
                var dto = DbContext.Find<PricedOrder>(@event.SourceId);
                if (WasNotAlreadyHandled(dto, @event.Version))
                {
                    dto.ReservationExpirationDate = null;
                    dto.OrderVersion = @event.Version;
                    DbContext.Save(dto);
                }
            }
        }

        public void Handle(OrderExpired @event)
        {
            // No need to keep this priced order alive if it is expired.
            using (var context = this.contextFactory.Create())
            {
                var pricedOrder = new PricedOrder { OrderId = @event.SourceId };
                var set = DbContext.Set<PricedOrder>();
                set.Attach(pricedOrder);
                set.Remove(pricedOrder);
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /*Trace.TraceWarning(
                        "Ignoring priced order expiration message with version {1} for order id {0}. This could be caused because the message was already handled and the entity was already deleted.",
                        pricedOrder.OrderId,
                        @event.Version);*/
                }
            }
        }

        /// <summary>
        /// Saves the seat assignments correlation ID for further lookup.
        /// </summary>
        public void Handle(AnchorAssignmentsCreated @event)
        {
            using (var context = this.contextFactory.Create())
            {
                var dto = DbContext.Find<PricedOrder>(@event.OrderId);
                dto.AssignmentsId = @event.SourceId;
                // Note: @event.Version does not correspond to order.Version.;
                context.SaveChanges();
            }
        }

        public void Handle(AnchorCreated @event)
        {
            using (var context = this.contextFactory.Create())
            {
                var dto = DbContext.Find<PricedOrderLineSeatTypeDescription>(@event.SourceId);
                if (dto == null)
                {
                    dto = new PricedOrderLineSeatTypeDescription { SeatTypeID = @event.SourceId };
                    DbContext.Set<PricedOrderLineSeatTypeDescription>().Add(dto);
                }

                dto.Name = @event.Name;
                context.SaveChanges();
            }
        }

        public void Handle(AnchorUpdated @event)
        {
            using (var context = this.contextFactory.Create())
            {
                var dto = DbContext.Find<PricedOrderLineSeatTypeDescription>(@event.SourceId);
                if (dto == null)
                {
                    dto = new PricedOrderLineSeatTypeDescription { SeatTypeID = @event.SourceId };
                    DbContext.Set<PricedOrderLineSeatTypeDescription>().Add(dto);
                }

                dto.Name = @event.Name;
                context.SaveChanges();
                this.seatDescriptionsCache.Set("SeatDescription_" + dto.SeatTypeID.ToString(), dto, DateTimeOffset.UtcNow.AddMinutes(5));
            }
        }

        private static bool WasNotAlreadyHandled(PricedOrder pricedOrder, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > pricedOrder.OrderVersion)
            {
                return true;
            }
            else if (eventVersion == pricedOrder.OrderVersion)
            {
                /*Trace.TraceWarning(
                    "Ignoring duplicate priced order update message with version {1} for order id {0}",
                    pricedOrder.OrderId,
                    eventVersion);*/
                return false;
            }
            else
            {
                /*Trace.TraceWarning(
                    @"Ignoring an older order update message was received with with version {1} for order id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order. Nevertheless, this warning can be expected in a migration scenario.",
                    pricedOrder.OrderId,
                    eventVersion,
                    pricedOrder.OrderVersion);*/
                return false;
            }
        } 
        
        private List<PricedOrderLineSeatTypeDescription> GetSeatTypeDescriptions(IEnumerable<Guid> seatTypeIds)
        {
            var result = new List<PricedOrderLineSeatTypeDescription>();
            var notCached = new List<Guid>();

            PricedOrderLineSeatTypeDescription cached;
            foreach (var seatType in seatTypeIds)
            {
                cached = (PricedOrderLineSeatTypeDescription)this.seatDescriptionsCache.Get("SeatDescription_" + seatType.ToString());
                if (cached == null)
                {
                    notCached.Add(seatType);
                }
                else
                {
                    result.Add(cached);
                }
            }

            if (notCached.Count > 0)
            {
                var notCachedArray = notCached.ToArray();
                var seatTypeDescriptions = DbContext.Query<PricedOrderLineSeatTypeDescription>()
                    .Where(x => notCachedArray.Contains(x.SeatTypeID))
                    .ToList();

                foreach (var seatType in seatTypeDescriptions)
                {
                    // even though we went got a fresh version we don't want to overwrite a fresher version set by the event handler for seat descriptions
                    var desc = seatDescriptionsCache.GetOrCreate(
                            "SeatDescription_" + seatType.SeatTypeID.ToString(), entry => {
                                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5);
                                return seatType;
                            });

                    result.Add(desc);
                }
            }

            return result;
        }
    }
}
