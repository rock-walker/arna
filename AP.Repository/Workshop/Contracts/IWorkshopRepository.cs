using AP.Core.GeoLocation;
using AP.EntityModel.AutoDomain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopRepository
    {
        Task<List<WorkshopData>> GetAll();
        IEnumerable<WorkshopData> GetBySlug(IEnumerable<string> ids);
        IEnumerable<WorkshopData> GetClosestLocations(GeoLocation[] rectangle, 
            double latitude, double longitude, double radius);
    }
}
