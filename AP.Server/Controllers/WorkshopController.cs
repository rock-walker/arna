using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Server.Application;
using Microsoft.AspNetCore.Authorization;

namespace AP.Application
{
    //[Authorize(Roles = "Client, Master, Administrator, PowerUser, Moderator")]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class WorkshopController : Controller
    {
        private readonly IWorkshopService _workshop;

        public WorkshopController(IWorkshopService workshop)
        {
            _workshop = workshop;
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await _workshop.GetAll();
        }

        public async Task<IEnumerable<WorkshopShortViewModel>> GetAround(double latitude, double longitude, double distance)
        {
            if (distance < 0.1)
            {
                await Task.FromException(new ArgumentException("distance very close"));
            }
            return await _workshop.GetByLocation(latitude, longitude,  distance);
        }

        public IEnumerable<WorkshopViewModel> GetById(
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))]
            IEnumerable<Guid> workshops)
        {
            return _workshop.GetById(workshops);
        }
    }
}