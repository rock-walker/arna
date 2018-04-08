using AP.Core.Model.User;
using AP.ViewModel.Workshop;
using System;
using System.Threading.Tasks;

namespace AP.Business.AutoPortal.Workshop.Contracts
{
    public interface IWorkshopAccountService
    {
        Task<Guid> Add(WorkshopAccountViewModel workshopViewModel, ApplicationUser user);
        void Update(WorkshopAccountViewModel workshopViewModel);
        Task CreateAnchor(Guid workshopId, AnchorTypeViewModel anchorView);
        void Publish(Guid workshopId);
        void Unpublish(Guid workshopId);
    }
}
