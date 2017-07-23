using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Business.AutoDomain.Workshop.Contracts
{
    public interface IWorkshopService
    {
        Task<IEnumerable<WorkshopViewModel>> GetByCity(string city);
        Task<IEnumerable<WorkshopViewModel>> GetById(IEnumerable<Guid> id);
        Task<IEnumerable<WorkshopViewModel>> GetAll();
    }
}
