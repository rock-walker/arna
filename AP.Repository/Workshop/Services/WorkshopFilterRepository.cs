using AP.Repository.Workshop.Contracts;
using System;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Context;
using AP.Core.Database;
using System.Linq;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopFilterRepository : AmbientContext<WorkshopContext>, IWorkshopFilterRepository
    {
        public WorkshopFilterRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public WorkshopData FindById(Guid id)
        {
            return DbContext.Find<WorkshopData>(id);
        }

        public WorkshopData FindBySlug(string slug)
        {
            return DbContext.Workshops.FirstOrDefault(x => x.Slug == slug);
        }

        public Task<WorkshopData> FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
