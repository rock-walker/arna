using AP.EntityModel.AutoDomain;
using System;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopAccountRepository
    {
        Task<Guid> Add(WorkshopData account);
        void Update(WorkshopData account);
    }
}
