using AP.ViewModel.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopRepository
    {
        Task<IEnumerable<WorkshopViewModel>> GetAll();
    }
}
