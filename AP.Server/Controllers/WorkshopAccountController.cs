using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.AutoPortal.Workshop.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace AP.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class WorkshopAccountController : Controller
    {
        private readonly IWorkshopAccountService _workshopAccountService;


        public WorkshopAccountController(IWorkshopAccountService workshopAccountService)
        {
            _workshopAccountService = workshopAccountService;
        }

        [HttpGet]
        public async Task<string> GetAccountPhone(string userId)
        {
            if (userId == null)
            {
                await Task.FromException(new ArgumentException("User ID is empty"));
            }

            return await _workshopAccountService.GetAccountPhone(userId);
        }

        [HttpGet]
        public async Task<WorkshopShortViewModel> FindByName(string name)
        {
            if (name == null || name.Length < 3)
            {
                await Task.FromException(new ArgumentException("Invalid workshop name"));
            }

            return await _workshopAccountService.FindByName(name);
        }

        [HttpPost]
        public async Task<WorkshopAccountResult> Add([FromBody]WorkshopAccountViewModel workshop)
        {
            if (ModelState.IsValid)
            {
                await _workshopAccountService.Add(workshop);
                return WorkshopAccountResult.WorkshopCreated;
            }

            return WorkshopAccountResult.WorkshopError;
        }

        [Produces("application/json")]
        public async Task Edit()
        {

        }
    }
}