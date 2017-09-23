using AP.Repository.Workshop.Contracts;
using System;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopAccountRepository : IWorkshopAccountRepository
    {
        private readonly IAmbientDbContextLocator _ambientLocator;

        private WorkshopContext DbContext
        {
            get
            {
                var dbContext = _ambientLocator.Get<WorkshopContext>();
                if (dbContext == null)
                {
                    throw new InvalidOperationException("No ambient DbContext of type WorkshopDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
                }
                return dbContext;
            }
        }

        public WorkshopAccountRepository(IAmbientDbContextLocator locator)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("ambientDbContextLocator");
            }

            _ambientLocator = locator;
        }

        public async Task Add(WorkshopData account)
        {
            await DbContext.AddAsync(account);
        }

        public Task Update(WorkshopData account)
        {
            throw new NotImplementedException();
        }
    }
}
