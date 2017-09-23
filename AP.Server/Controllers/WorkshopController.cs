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
    [Route("api/[controller]/[action]")]
    public class WorkshopController : Controller
    {
        private readonly IWorkshopService _workshop;

        public WorkshopController(IWorkshopService workshop)
        {
            _workshop = workshop;
        }

        public async Task<IEnumerable<WorkshopViewModel>> GetByCity(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("City is empty");
            }
            return await _workshop.GetByCity(name);
        }

        [AllowAnonymous]
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAll()
        {
            return await _workshop.GetAll();
        }

        [AllowAnonymous]
        public async Task<IEnumerable<WorkshopShortViewModel>> GetAround(double latitude, double longitude, double distance)
        {
            if (distance < 0.1)
            {
                await Task.FromException(new ArgumentException("distance very close"));
            }
            return await _workshop.GetByLocation(latitude, longitude,  distance);
        }

        [Route("id")]
        public async Task<IEnumerable<WorkshopViewModel>> GetById(
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))]
            IEnumerable<Guid> workshops)
        {
            return await _workshop.GetById(workshops);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}