using AP.Core.GeoLocation;
using AP.EntityModel.AutoDomain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopRepository
    {
        Task<IEnumerable<WorkshopData>> GetAll();
        IEnumerable<WorkshopData> GetBySlug(IEnumerable<string> ids);
        Task<IEnumerable<WorkshopData>> GetClosestLocations(GeoLocation[] rectangle, 
            double latitude, double longitude, double radius);
    }
}
