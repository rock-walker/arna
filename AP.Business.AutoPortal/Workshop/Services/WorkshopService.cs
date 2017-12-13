using AP.ViewModel.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Repository.Workshop.Contracts;
using AP.Core.GeoLocation;
using EntityFramework.DbContextScope.Interfaces;
using AutoMapper;
using AP.Infrastructure.Messaging;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopService : IWorkshopService
    {
        private const int EarthRadius = 6371;
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly IEventBus eventBus;

        public WorkshopService(IDbContextScopeFactory scope, IWorkshopRepository repository, IEventBus eventBus)
        {
            _workshopRepo = repository;
            _dbContextScope = scope;
            this.eventBus = eventBus;
        }
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await Task.Run(() => 
                Mapper.Map<IEnumerable<WorkshopShortViewModel>>(_workshopRepo.GetAll())
            );
        }

        public IEnumerable<WorkshopViewModel> GetBySlug(IEnumerable<string> ids)
        {
            using (var scope = _dbContextScope.CreateReadOnly())
            {
                var workshops = _workshopRepo.GetBySlug(ids);
                var mappedWorkshops = Mapper.Map<IEnumerable<WorkshopViewModel>>(workshops);

                return mappedWorkshops;
            }
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetByLocation(double latitude, double longitute, double distance)
        {
            var geoLocation = GeoLocation.FromDegrees(latitude, longitute);
            var boundingCoordinates = geoLocation.BoundingCoordinates(distance);
            var radius = distance / EarthRadius;

            return await Task.Run(() =>
            {
                var locations = _workshopRepo.GetClosestLocations(boundingCoordinates,
                    geoLocation.getLatitudeInRadians(),
                    geoLocation.getLongitudeInRadians(),
                    radius);

                return Mapper.Map<IEnumerable<WorkshopShortViewModel>>(locations);
            });
        }
    }
}
