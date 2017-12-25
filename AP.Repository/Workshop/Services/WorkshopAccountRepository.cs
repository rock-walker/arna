using AP.Repository.Workshop.Contracts;
using System;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using AP.Core.Database;
using AP.EntityModel.Booking;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopAccountRepository : AmbientContext<WorkshopContext>, IWorkshopAccountRepository
    {
        public WorkshopAccountRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public async Task<Guid> Add(WorkshopData account)
        {
            await DbContext.AddAsync(account);
            return account.ID;
        }

        public void Update(WorkshopData @new, WorkshopData source)
        {
            DbContext.Entry(source).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            DbContext.Update(@new);
        }

        public WorkshopData LoadAnchors(WorkshopData account)
        {
            DbContext.Entry(account).Collection(p => p.Anchors).Load();
            return account;
        }

        public WorkshopData LoadAddress(WorkshopData account)
        {
            DbContext.Entry(account).Reference(p => p.Address).Load();
            DbContext.Entry(account.Address).Reference(p=>p.City).Load();

            return account;
        }

        public bool Exists(Guid id)
        {
            var data = DbContext.Find<WorkshopData>(id);
            return data != null;
        }

        public async Task CreateAnchor(AnchorType anchor)
        {
            await DbContext.AddAsync(anchor);
        }
    }
}
