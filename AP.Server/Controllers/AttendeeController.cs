using System;
using Microsoft.AspNetCore.Mvc;
using AP.ViewModel.Attendee;
using AP.Business.Attendee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using System.Threading.Tasks;
using AP.Shared.Security.Contracts;

namespace AP.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Verified")]
    public class AttendeeController : IdentityController
    {
        private readonly IAttendeeAccountService attendeeService;
        private readonly IAccountService accountService;

        public AttendeeController(IAttendeeAccountService attendeeService,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService) : base(userManager)
        {
            this.attendeeService = attendeeService;
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<JwtResponse> Create([FromBody]AttendeeAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new JwtResponse
                {
                    Message = "Post model is invalid"
                };
            }

            var user = await GetCurrentUser();

            var id = await attendeeService.Register(model, user);
            if (id != Guid.Empty)
            {
                return await accountService.RefreshJwt(user, model.RefreshToken);
            }

            return new JwtResponse
            {
                Message = "Creation of attendee failed"
            };
        }

        public StatusCodeResult Test()
        {
            attendeeService.Test();
            return Ok();
        }
    }
}