using System;
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
        private readonly IWorkshopFilterService _filterService;


        public WorkshopAccountController(IWorkshopAccountService workshopAccountService,
                                         IWorkshopFilterService filterService)
        {
            _workshopAccountService = workshopAccountService;
            _filterService = filterService;
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

            return await _filterService.FindByName(name);
        }

        [HttpPost]
        public async Task<string> Add([FromBody]WorkshopAccountViewModel workshop)
        {
            if (ModelState.IsValid)
            {
                var workshopId = await _workshopAccountService.Add(workshop);
                return workshopId.ToString();
            }

            return WorkshopAccountResult.WorkshopError.ToString();
        }

        [HttpPost]
        public async Task<WorkshopAccountResult> Edit([FromBody]WorkshopAccountViewModel workshop)
        {
            await _workshopAccountService.Update(workshop);
            return WorkshopAccountResult.WorkshopUpdated;
         }
    }
}