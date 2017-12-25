using AP.ViewModel.Workshop;
using System;
using System.Threading.Tasks;

namespace AP.Business.AutoPortal.Workshop.Contracts
{
    public interface IWorkshopAccountService
    {
        Task<string> GetAccountPhone(string userId);
        Task<Guid> Add(WorkshopAccountViewModel workshopViewModel);
        void Update(WorkshopAccountViewModel workshopViewModel);
        Task CreateAnchor(Guid workshopId, AnchorTypeViewModel anchorView);
        void Publish(Guid workshopId);
        void Unpublish(Guid workshopId);
    }
}
