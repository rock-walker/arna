namespace AP.Business.Registration.ReadModel.Implementation
{
    using AP.Core.Database;
    using AP.EntityModel.ReadModel;
    using AP.Repository.Context;
    using EntityFramework.DbContextScope.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WorkshopDao : AmbientContext<WorkshopRegistrationDbContext>, IWorkshopDao
    {
        public WorkshopDao(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public WorkshopDetails GetWorkshopDetails(string workshopCode)
        {
            return DbContext
                .Query<WorkshopView>()
                .Where(dto => dto.Code == workshopCode)
                .Select(x => new WorkshopDetails
                {
                    Id = x.ID,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Location = x.Location,
                    Tagline = x.Tagline,
                    TwitterSearch = x.TwitterSearch,
                    StartDate = x.StartDate
                })
                .FirstOrDefault();
        }

        public WorkshopAlias GetWorkshopAlias(string workshopCode)
        {
            return DbContext
                .Query<WorkshopView>()
                .Where(dto => dto.Code == workshopCode)
                .Select(x => new WorkshopAlias { Id = x.ID, Code = x.Code, Name = x.Name, Tagline = x.Tagline })
                .FirstOrDefault();
        }

        public IList<WorkshopAlias> GetPublishedWorkshops()
        {
            return DbContext
                .Query<WorkshopView>()
                .Where(dto => dto.IsPublished)
                .Select(x => new WorkshopAlias { Id = x.ID, Code = x.Code, Name = x.Name, Tagline = x.Tagline })
                .ToList();
        }

        public IList<AnchorType> GetPublishedAnchorTypes(Guid workshopId)
        {
             return DbContext.Query<AnchorType>()
                .Where(c => c.WorkshopID == workshopId)
                .ToList();
        }

        public IList<AnchorTypeName> GetAnchorTypeNames(IEnumerable<Guid> seatTypes)
        {
            var distinctIds = seatTypes.Distinct().ToArray();
            if (distinctIds.Length == 0)
                return new List<AnchorTypeName>();

            return DbContext.Query<AnchorType>()
                .Where(x => distinctIds.Contains(x.ID))
                .Select(s => new AnchorTypeName { ID = s.ID, Name = s.Name })
                .ToList();
        }
    }
}