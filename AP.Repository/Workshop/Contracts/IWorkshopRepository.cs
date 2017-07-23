using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopRepository
    {
        Task<IEnumerable<WorkshopViewModel>> GetAll();
        Task<IEnumerable<WorkshopViewModel>> GetById(IEnumerable<Guid> ids);
    }
}
