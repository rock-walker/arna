using AP.Shared.Security.Contracts;
using AP.Core.Model;
using AP.Core.Model.User;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AP.Shared.Security.Services
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        ILogger<IdentityProvider> logger;

        public IdentityProvider(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<IdentityProvider> logger)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<Tuple<ApplicationUser, IdentityStatus, IList<string>>> Login(LoginInfo info)
        {
            var user = await userManager.FindByEmailAsync(info.User);
            if (user == null)
            {
                logger.LogWarning($"User {info.User} doesn't exist.");
                return Tuple.Create<ApplicationUser, IdentityStatus, IList<string>>(null, IdentityStatus.Error, null);
            }

            var roles = await userManager.GetRolesAsync(user);

            var result = await signInManager.CheckPasswordSignInAsync(user, info.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                logger.LogInformation($"User {info.User} is logged in successfully.");
                return Tuple.Create(user, IdentityStatus.LoggedInSuccess, roles);
            }
            if (result.RequiresTwoFactor)
            {
                return Tuple.Create<ApplicationUser, IdentityStatus, IList<string>>(null, IdentityStatus.TwoFactorRequiresError, null);
                //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                logger.LogWarning($"User {info.User} account locked out.");
                return Tuple.Create<ApplicationUser, IdentityStatus, IList<string>>(null, IdentityStatus.LockedOutPassordError, null);
            }

            return Tuple.Create<ApplicationUser, IdentityStatus, IList<string>>(null, IdentityStatus.Error, null);
        }
    }
}
