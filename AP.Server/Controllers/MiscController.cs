using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AP.Shared.Security.Contracts;
using System.Threading.Tasks;
using System;
using AP.Core.Model.User;
using Microsoft.Extensions.Localization;

namespace AP.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class MiscController : IdentityController
    {
        private readonly IAccountService accountService;
        private readonly IStringLocalizer<MiscController> localizer;

        public MiscController(IAccountService accountService,
            IStringLocalizer<MiscController> localizer) : base(accountService)
        {
            this.accountService = accountService;
            this.localizer = localizer;
        }

        [HttpPost("set-client-type")]
        public async Task<JwtResponse> SetClientType(int role)
        {
            var typedRole = (Roles)role;
            if (Enum.IsDefined(typeof(Roles), typedRole))
            {
                if (typedRole == Roles.Client || typedRole == Roles.Master)
                {
                    var user = await GetCurrentUser();
                    await accountService.AddRole(user, typedRole);
                    return await accountService.RefreshJwt(user);
                }
                else
                {
                    throw new ArgumentException(localizer["InvalidClientTypeEx"]);
                }
            }

            throw new ArgumentException(localizer["InvalidClientTypeEx"]);
        }
    }
}