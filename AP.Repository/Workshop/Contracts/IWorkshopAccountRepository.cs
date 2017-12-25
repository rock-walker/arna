using AP.EntityModel.AutoDomain;
using AP.EntityModel.Booking;
using System;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopAccountRepository
    {
        Task<Guid> Add(WorkshopData account);
        void Update(WorkshopData @new, WorkshopData source);
        Task CreateAnchor(AnchorType anchor);
        WorkshopData LoadAnchors(WorkshopData account);
        WorkshopData LoadAddress(WorkshopData account);
    }
}
