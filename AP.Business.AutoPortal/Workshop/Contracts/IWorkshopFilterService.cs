using AP.ViewModel.Workshop;
using System;
using System.Threading.Tasks;

namespace AP.Business.AutoPortal.Workshop.Contracts
{
    public interface IWorkshopFilterService
    {
        Task<WorkshopViewModel> FindById(Guid id);
        Task<WorkshopShortViewModel> FindByName(string name);
    }
}
