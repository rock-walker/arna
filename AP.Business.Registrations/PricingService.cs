namespace AP.Business.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using AP.Business.Registration.ReadModel;
    using AP.Business.Model.Registration;
    using EntityFramework.DbContextScope.Interfaces;
    using AP.Core.Database;
    using AP.Repository.Context;

    public class PricingService : AmbientContext<WorkshopRegistrationDbContext>, IPricingService
    {
        private readonly IWorkshopDao workshopDao;
        private readonly IDbContextScopeFactory factory;

        public PricingService(IWorkshopDao workshopDao, IDbContextScopeFactory factory, IAmbientDbContextLocator locator) : base(locator)
        {
            if (workshopDao == null) throw new ArgumentNullException("workshopDao");

            this.workshopDao = workshopDao;
            this.factory = factory;
        }

        public OrderTotal CalculateTotal(Guid workshopId, ICollection<AnchorQuantity> seatItems)
        {
            using (var scope = factory.CreateReadOnly())
            {
                var seatTypes = workshopDao.GetPublishedAnchorTypes(workshopId);
                var lineItems = new List<OrderLine>();
                foreach (var item in seatItems)
                {
                    var seatType = seatTypes.FirstOrDefault(x => x.ID == item.AnchorType);
                    if (seatType == null)
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Invalid anchor type ID '{0}' for workshop with ID '{1}'", item.AnchorType, workshopId));
                    }

                    lineItems.Add(new SeatOrderLine { SeatType = item.AnchorType, Quantity = item.Quantity, UnitPrice = seatType.Price, LineTotal = Math.Round(seatType.Price * item.Quantity, 2) });
                }

                return new OrderTotal
                {
                    Total = lineItems.Sum(x => x.LineTotal),
                    Lines = lineItems
                };
            }
        }
    }
}