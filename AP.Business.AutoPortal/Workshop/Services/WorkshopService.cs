using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Repository.Workshop.Contracts;
using AP.Core.GeoLocation;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopService : IWorkshopService
    {
        private const int EarthRadius = 6371;
        private readonly IWorkshopRepository _repo;

        public WorkshopService(IWorkshopRepository repository)
        {
            _repo = repository;
        }
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await Task.Run(() => _repo.GetAll());
        }

        public async Task<IEnumerable<WorkshopViewModel>> GetById(IEnumerable<Guid> ids)
        {
            return await Task.Run(() => _repo.GetById(ids));
        }

        public Task<IEnumerable<WorkshopViewModel>> GetByCity(string city)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetByLocation(double latitude, double longitute, double distance)
        {
            var geoLocation = GeoLocation.FromDegrees(latitude, longitute);
            var boundingCoordinates = geoLocation.BoundingCoordinates(distance);
            var radius = distance / EarthRadius;

            return await Task.Run(() => 
                _repo.GetClosestLocations(boundingCoordinates, geoLocation.getLatitudeInRadians(), geoLocation.getLongitudeInRadians(), radius)
            );
        }
    }
}
