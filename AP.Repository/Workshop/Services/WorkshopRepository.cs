using AP.ViewModel.Workshop;
using AP.EntityModel.Mappers;
using AP.Repository.Context;
using AP.Repository.Workshop.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Core.GeoLocation;
using Microsoft.Extensions.Configuration;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopRepository : IWorkshopRepository
    {
        private readonly WorkshopContext _ctx;
        private object _lockObject = new object();

        public WorkshopRepository(WorkshopContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await Task.Run(() =>
            {
                return QueryAllShortWorkshops().Select(x => x.MapToShort());
            });
        }

        public async Task<IEnumerable<WorkshopViewModel>> GetById(IEnumerable<Guid> ids)
        {
            return await Task.Run(() =>
            {
                var workshops = QueryAllWorkshops();
                var selectedWorkshops = new List<WorkshopViewModel>();
                foreach (var id in ids)
                {
                    var w = workshops.Single(x => x.ID == id).MapTo();
                    selectedWorkshops.Add(w);
                }

                return selectedWorkshops;
            });
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetClosestLocations(
            GeoLocation[] rectangle, double latitude, double longitude, double radius)
        {
            return await Task.Run(() =>
            {
                var workshops = _ctx.Workshops
                                .Include(x => x.Location)
                                .AsNoTracking();

                var locations = _ctx.Locations.FromSql("GetClosestLocations @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                    rectangle[0].getLatitudeInRadians(),
                    rectangle[0].getLongitudeInRadians(),
                    rectangle[1].getLatitudeInRadians(),
                    rectangle[1].getLongitudeInRadians(),

                    latitude,
                    longitude,
                    radius).ToList();

                var filtered = from w in workshops
                               join l in locations
                               on w.LocationID equals l.ID
                               select w;

                return filtered.Select(x => x.MapToShort());
            });
        }

        private IQueryable<EntityModel.AutoDomain.Workshop> QueryAllWorkshops()
        {
            lock (_lockObject)
            {
                return _ctx.Workshops
                        .Include(x => x.WorkshopCategories)
                            .ThenInclude(x => x.Category)
                        .Include(x => x.Contact)
                        .Include(x => x.AutoBrand)
                        .Include(x => x.Address)
                            .ThenInclude(x => x.City)
                        .Include(x => x.Location)
                        .Include(x => x.Logo)
                        .AsNoTracking();
            }
        }

        private EntityModel.AutoDomain.Workshop QueryShortWorkshops(Func<EntityModel.AutoDomain.Workshop, bool> condition)
        {
            lock (_lockObject)
            {
                return _ctx.Workshops
                        .Include(x => x.Location)
                        .Include(x => x.AutoBrand)
                        .AsNoTracking()
                        .Where(condition)
                        .FirstOrDefault();
            }
        }

        private IQueryable<EntityModel.AutoDomain.Workshop> QueryAllShortWorkshops()
        {
            lock (_lockObject)
            {
                return _ctx.Workshops
                        .Include(x => x.AutoBrand)
                        .Include(x => x.Location)
                        .AsNoTracking();
            }
        }
    }
}
