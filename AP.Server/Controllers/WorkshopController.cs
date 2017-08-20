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
    [AllowAnonymous]
    [Route("api/[controller]")]
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

        [Route("all")]
        public async Task<IEnumerable<WorkshopViewModel>> Get()
        {
            return await _workshop.GetAll();
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