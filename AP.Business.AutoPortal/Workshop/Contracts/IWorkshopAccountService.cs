using AP.ViewModel.Workshop;
using System;
using System.Threading.Tasks;

namespace AP.Business.AutoPortal.Workshop.Contracts
{
    public interface IWorkshopAccountService
    {
        Task<string> GetAccountPhone(string userId);
        Task<Guid> Add(WorkshopAccountViewModel workshopViewModel);
        Task Update(WorkshopAccountViewModel workshopViewModel);
    }
}
