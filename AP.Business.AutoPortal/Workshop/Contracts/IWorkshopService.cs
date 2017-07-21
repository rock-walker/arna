using AP.ViewModel.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Business.AutoDomain.Workshop.Contracts
{
    public interface IWorkshopService
    {
        Task<IEnumerable<WorkshopViewModel>> GetByCity(string city);
        Task<IEnumerable<WorkshopViewModel>> GetAll();
    }
}
