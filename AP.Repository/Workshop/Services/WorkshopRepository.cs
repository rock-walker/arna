﻿using AP.Repository.Context;
using AP.Repository.Workshop.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Core.GeoLocation;
using EntityFramework.DbContextScope.Interfaces;
using AP.EntityModel.AutoDomain;
using AP.Core.Database;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopRepository : AmbientContext<WorkshopContext>, IWorkshopRepository
    {
        public WorkshopRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public Task<List<WorkshopData>> GetAll()
        {
            return Task.Run(() =>
            {
                return QueryAllShortWorkshops().ToList();
            });
        }

        public IEnumerable<WorkshopData> GetBySlug(IEnumerable<string> ids)
        {
            var workshops = QueryFullWorkshop();
            var selectedWorkshops = new List<WorkshopData>();
            foreach (var id in ids)
            {
                var w = workshops.FirstOrDefault(x => x.Slug == id);
                if (w != null)
                {
                    selectedWorkshops.Add(w);
                }
            }

            return selectedWorkshops;
        }

        public IEnumerable<WorkshopData> GetClosestLocations(
            GeoLocation[] rectangle, double latitude, double longitude, double radius)
        {
            var workshops = DbContext.Workshops
                            .Include(x => x.Location)
                            .AsNoTracking();

            var locations = DbContext.Locations.FromSql("GetClosestLocations @p0, @p1, @p2, @p3, @p4, @p5, @p6",
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

            return filtered;//.Select(x => x.MapToShort());
        }

        private IQueryable<WorkshopData> QueryFullWorkshop()
        {
            return DbContext.Workshops
                    .Include(x => x.WorkshopCategories)
                        //.ThenInclude(x => x.Category)
                    .Include(x => x.Contact)
                    .Include(x => x.Address)
                        .ThenInclude(x => x.City)
                    .Include(x => x.Location)
                    .Include(x => x.Logo)
                    .Include(x => x.WorkshopAutobrands)
                    .Include(x => x.WorkshopWeekTimetable)
                    .AsNoTracking();
        }

        private WorkshopData QueryShortWorkshops(Func<WorkshopData, bool> condition)
        {
            return DbContext.Workshops
                    .Include(x => x.Location)
                    .AsNoTracking()
                    .Where(condition)
                    .FirstOrDefault();
        }

        private IQueryable<WorkshopData> QueryAllShortWorkshops()
        {
            return DbContext.Workshops
                    .Include(x => x.Location)
                    .AsNoTracking();
        }
    }
}
