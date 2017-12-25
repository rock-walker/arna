using System;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Attendee;
using AP.Business.Attendee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using System.Threading.Tasks;

namespace AP.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class AttendeeController : IdentityController
    {
        private readonly IAttendeeAccountService attendeeService;

        public AttendeeController(IAttendeeAccountService attendeeService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            this.attendeeService = attendeeService;
        }

        [HttpPost]
        public async Task<StatusCodeResult> Create([FromBody]AttendeeAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await GetCurrentUser();

            var id = attendeeService.Register(model, user.Id);
            if (id != Guid.Empty)
            {
                return Ok();
            }

            return NoContent();
        }

        public StatusCodeResult Test()
        {
            attendeeService.Test();
            return Ok();
        }
    }
}