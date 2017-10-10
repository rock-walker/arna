using AP.Core.GeoLocation;
using AP.EntityModel.AutoDomain;
using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopRepository
    {
        Task<IEnumerable<WorkshopShortViewModel>> GetAll();
        IEnumerable<WorkshopData> GetById(IEnumerable<Guid> ids);
        Task<IEnumerable<WorkshopShortViewModel>> GetClosestLocations(GeoLocation[] rectangle, 
            double latitude, double longitude, double radius);
    }
}
