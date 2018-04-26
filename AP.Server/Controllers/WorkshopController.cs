using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Server.Application;
using Microsoft.AspNetCore.Authorization;
using AP.Server;
using AP.Business.Registration.ReadModel;
using System.Linq;
using EntityFramework.DbContextScope.Interfaces;

namespace AP.Application
{
    //[Authorize(Roles = "Client, Master, Administrator, PowerUser, Moderator")]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class WorkshopController : WorkshopTenantController
    {
        private readonly IWorkshopService workshop;
        private readonly IDbContextScopeFactory factory;

        public WorkshopController(IWorkshopService workshop, IWorkshopDao workshopDao, IDbContextScopeFactory factory) : base(workshopDao)
        {
            this.workshop = workshop;
            this.factory = factory;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await workshop.GetAll();
        }

        [HttpGet("around")]
        public IEnumerable<WorkshopShortViewModel> GetAround(double latitude, double longitude, double distance)
        {
            if (distance < 0.1)
            {
                throw new ArgumentException("distance very close");
            }

            return workshop.GetByLocation(latitude, longitude,  distance);
        }
        
        [HttpGet("get-by-code")]
        public IEnumerable<WorkshopViewModel> GetByCode(
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))]
            IEnumerable<string> workshops)
        {
            return workshop.GetBySlug(workshops);
        }
        
        [HttpGet("/api/{workshopCode}/")]
        public WorkshopViewModel GetByCode()
        {
            using (var scope = factory.CreateReadOnly())
            {
                var alias = WorkshopAlias;
                if (alias != null)
                {
                    return workshop.GetBySlug(new[] { WorkshopCode }).FirstOrDefault();
                }
            }

            return null;
        }
    }
}