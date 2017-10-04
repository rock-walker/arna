using AP.Repository.Workshop.Contracts;
using System;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Base;
using AP.Repository.Context;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopFilterRepository : AmbientContext<WorkshopContext>, IWorkshopFilterRepository
    {
        private readonly IAmbientDbContextLocator _ambientLocator;

        public WorkshopFilterRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public async Task<WorkshopData> FindById(Guid id)
        {
            var task = await DbContext.FindAsync<WorkshopData>(id);
            DbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            return task;
        }

        public Task<WorkshopData> FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
