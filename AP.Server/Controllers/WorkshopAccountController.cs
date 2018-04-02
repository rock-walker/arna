using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Workshop;
using AP.Business.AutoPortal.Workshop.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AP.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class WorkshopAccountController : Controller
    {
        private readonly IWorkshopAccountService workshopAccountService;
        private readonly IWorkshopFilterService filterService;

        private WorkshopViewModel Workshop { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var slug = (string)ControllerContext.RouteData.Values["slug"];
            if (!string.IsNullOrEmpty(slug))
            {
                Workshop = filterService.FindBySlug(slug);
                if (Workshop == null)
                {
                    filterContext.Result = new UnauthorizedResult();
                }
                /*
                if (Workshop != null)
                {
                    var accessCode = (string)ControllerContext.RouteData.Values["accessCode"];

                    if (accessCode == null || !string.Equals(accessCode, this.Workshop.AccessCode, StringComparison.Ordinal))
                    {
                        filterContext.Result = new UnauthorizedResult();
                    }
                }
                */
            }

            base.OnActionExecuting(filterContext);
        }

        public WorkshopAccountController(IWorkshopAccountService workshopAccountService,
                                         IWorkshopFilterService filterService)
        {
            this.workshopAccountService = workshopAccountService;
            this.filterService = filterService;
        }

        [HttpGet]
        public async Task<string> GetAccountPhone(string userId)
        {
            if (userId == null)
            {
                await Task.FromException(new ArgumentException("User ID is empty"));
            }

            return await workshopAccountService.GetAccountPhone(userId);
        }

        [HttpGet]
        public async Task<WorkshopShortViewModel> FindByName(string name)
        {
            if (name == null || name.Length < 3)
            {
                await Task.FromException(new ArgumentException("Invalid workshop name"));
            }

            return await filterService.FindByName(name);
        }

        [HttpPost]
        public async Task<string> Add([FromBody]WorkshopAccountViewModel workshop)
        {
            if (ModelState.IsValid)
            {
                workshop.RegisterDate = DateTime.UtcNow;
                var workshopId = await workshopAccountService.Add(workshop);

                return workshopId.ToString();
            }

            return WorkshopAccountResult.WorkshopError.ToString();
        }

        [HttpPut]
        public WorkshopAccountResult Edit([FromBody]WorkshopAccountViewModel workshop)
        {
            if (ModelState.IsValid)
            {
                workshopAccountService.Update(workshop);
                return WorkshopAccountResult.WorkshopUpdated;
            }
            return WorkshopAccountResult.WorkshopError;
        }

        [Route("/{slug}/[action]")]
        public StatusCodeResult Publish()
        {
            try
            {
                workshopAccountService.Publish(Workshop.ID);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [Route("/{slug}/[action]")]
        public StatusCodeResult Unpublish()
        {
            try
            {
                workshopAccountService.Unpublish(Workshop.ID);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<int> CreateAnchor(string id, [FromBody]AnchorTypeViewModel anchorView)
        {
            if (ModelState.IsValid)
            {
                var workshopId = Guid.Parse(id);
                //TODO: verify here, does workshop exist or not

                await workshopAccountService.CreateAnchor(workshopId, anchorView);
                return 1;
            }
            return (int)WorkshopAccountResult.WorkshopError;
        }
    }
}