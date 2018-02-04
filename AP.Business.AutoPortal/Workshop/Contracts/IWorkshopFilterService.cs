using AP.ViewModel.Workshop;
using System;
using System.Threading.Tasks;

namespace AP.Business.AutoPortal.Workshop.Contracts
{
    public interface IWorkshopFilterService
    {
        WorkshopViewModel FindById(Guid id);
        WorkshopViewModel FindBySlug(string slug);
        Task<WorkshopShortViewModel> FindByName(string name);
    }
}
