using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using System.Security;

namespace AP.Server.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        protected async Task<ApplicationUser> GetCurrentUser()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

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
