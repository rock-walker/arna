using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AP.AutoDomain.Workshop;
using AP.DataContract;

namespace AP.Application
{
    public class WorkshopController : ApiController
    {
        private readonly IWorkshopService _workshop;

        public WorkshopController(IWorkshopService workshop)
        {
            _workshop = workshop;
        }

        public async Task<IEnumerable<Workshop>> GetByCity(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("City is empty");
            }
            return await _workshop.GetByCity(name);
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
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