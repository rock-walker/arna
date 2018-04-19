using AP.ViewModel.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Repository.Workshop.Contracts;
using AP.Core.GeoLocation;
using EntityFramework.DbContextScope.Interfaces;
using AutoMapper;
using AP.Repository.Common.Contracts;
using System.Linq;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopService : IWorkshopService
    {
        private const int EarthRadius = 6371;
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly ICategoryRepository categoryRepo;

        public WorkshopService(IDbContextScopeFactory scope, 
            IWorkshopRepository repository,
            ICategoryRepository categoryRepo)
        {
            _workshopRepo = repository;
            _dbContextScope = scope;
            this.categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            using (var scope = _dbContextScope.CreateReadOnly())
            {
                var workshopsData = await _workshopRepo.GetAll();

                return Mapper.Map<IEnumerable<WorkshopShortViewModel>>(workshopsData);
            }
        }

        public IEnumerable<WorkshopViewModel> GetBySlug(IEnumerable<string> ids)
        {
            using (var scope = _dbContextScope.CreateReadOnly())
            {
                //TODO: bullshit code, needs refactoring - 
                //root cause: need to merge categories from 2 contexts

                var workshops = _workshopRepo.GetBySlug(ids).ToList();
                if (workshops.Any())
                {
                    var dbCategories = workshops.Select(x =>
                    {
                        var workshopCategories = x.WorkshopCategories.Select(y => y.CategoryID);
                        return categoryRepo.Get(workshopCategories);
                    }).ToList();

                    if (dbCategories.Count != 0)
                    {
                        for (var i = 0; i < dbCategories.Count; i++)
                        {
                            var wCategories = workshops[i].WorkshopCategories.ToList();
                            var index = 0;
                            foreach (var c in dbCategories[i])
                            {
                                wCategories[index++].Category = c;
                            }
                        }
                    }
                }

                return Mapper.Map<IEnumerable<WorkshopViewModel>>(workshops);
            }
        }

        public IEnumerable<WorkshopShortViewModel> GetByLocation(double latitude, double longitute, double distance)
        {
            var geoLocation = GeoLocation.FromDegrees(latitude, longitute);
            var boundingCoordinates = geoLocation.BoundingCoordinates(distance);
            var radius = distance / EarthRadius;

            using (var scope = _dbContextScope.CreateReadOnly())
            {
                var locations = _workshopRepo.GetClosestLocations(boundingCoordinates,
                    geoLocation.getLatitudeInRadians(),
                    geoLocation.getLongitudeInRadians(),
                    radius);

                return Mapper.Map<IEnumerable<WorkshopShortViewModel>>(locations);
            }
        }
    }
}
