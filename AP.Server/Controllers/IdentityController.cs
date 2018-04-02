using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using System.Security;
using System.Security.Claims;
using System.Collections.Generic;

namespace AP.Server.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private IEnumerable<Claim> Claims => User.Claims;

        public IdentityController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        protected async Task<ApplicationUser> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
#if DEBUG
                throw new SecurityException("User not found");
#endif
                user = new ApplicationUser();
            }

            return user;
        }
    }
}
