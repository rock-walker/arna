using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AP.Core.Model.User;
using System.Security;
using System.Security.Claims;
using AP.Shared.Security.Contracts;

namespace AP.Server.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IAccountService accountService;

        public IdentityController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        protected async Task<ApplicationUser> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await accountService.FindByEmail(email);

            if (user == null)
            {
#if DEBUG
                throw new SecurityException("User not found");
#endif
                user = new ApplicationUser();
            }

            return user;
        }

        protected async Task<JwtResponse> RefreshJwt(ApplicationUser user = null)
        {
            if (user == null)
            {
                user = await GetCurrentUser();
            }

            return await accountService.RefreshJwt(user);
        }
    }
}
