using AP.Repository.Workshop.Contracts;
using System;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Context;
using AP.Core.Database;

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

        public Task<WorkshopData> FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
