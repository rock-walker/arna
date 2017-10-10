using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Repository.Workshop.Contracts;
using AP.Core.GeoLocation;
using EntityFramework.DbContextScope.Interfaces;
using AutoMapper;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopService : IWorkshopService
    {
        private const int EarthRadius = 6371;
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IWorkshopRepository _workshopRepo;

        public WorkshopService(IDbContextScopeFactory scope, IWorkshopRepository repository)
        {
            _workshopRepo = repository;
            _dbContextScope = scope;
        }
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await Task.Run(() => _workshopRepo.GetAll());
        }

        public IEnumerable<WorkshopViewModel> GetById(IEnumerable<Guid> ids)
        {
            using (var scope = _dbContextScope.CreateReadOnly())
            {
                var workshops = _workshopRepo.GetById(ids);
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
                _workshopRepo.GetClosestLocations(boundingCoordinates, geoLocation.getLatitudeInRadians(), geoLocation.getLongitudeInRadians(), radius)
            );
        }
    }
}
