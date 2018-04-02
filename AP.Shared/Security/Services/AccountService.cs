using AP.Shared.Security.Contracts;
using System;
using AP.Core.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AP.Core.Model;
using AP.Core.User.Authentication;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AP.Shared.Security.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IIdentityProvider identityProvider;
        private readonly IConfigurationRoot configuration;
        private readonly ILogger<AccountService> logger;

        public AccountService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IIdentityProvider identityProvider,
            IConfigurationRoot config,
            ILogger<AccountService> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.identityProvider = identityProvider;
            configuration = config;
            this.logger = logger;
        }

        public async Task AddRole(ApplicationUser user, Roles role)
        {
            var roleName = Enum.GetName(typeof(Roles), role);
            await userManager.AddToRoleAsync(user, roleName);
            logger.LogInformation($"Role {roleName} added to User {user.UserName} successfully.");
        }

        public async Task<ApplicationUser> FindUserById(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<JwtResponse> CompleteUserVerification(ApplicationUser user, string phoneNumber, string code)
        {
            var result = await userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            if (result.Succeeded)
            {
                //CHECK: looks like I don't need it anymore
                //await signInManager.SignInAsync(user, isPersistent: false);
                await AddRole(user, Roles.Verified);

                var verifiedRole = Enum.GetName(typeof(Roles), Roles.Verified);
                var jwtIdentity = new JwtIdentity
                {
                    User = user,
                    LoggedInStatus = IdentityStatus.LoggedInSuccess,
                    Roles = new string[] { verifiedRole }
                };

                var refreshToken = identityProvider.GenerateRefreshToken(jwtIdentity);
                jwtIdentity.RefreshToken = refreshToken;

                var options = JwtTokenProducer.InitializeOptions(configuration);
                return JwtTokenProducer.Produce(jwtIdentity, options);
            }

            logger.LogError($"Failed to complete user {user.UserName} verification with ex: {result.Errors.First()}.");
            return null;
        }

        public async Task<JwtResponse> RefreshJwt(ApplicationUser user, string verifyRefreshToken)
        {
            var roles = await userManager.GetRolesAsync(user);

            var jwtIdentity = new JwtIdentity
            {
                User = user,
                LoggedInStatus = IdentityStatus.LoggedInSuccess,
                Roles = roles
            };

            var jwtIdentityRf = await identityProvider.GetRefreshToken(verifyRefreshToken);

            jwtIdentity.RefreshToken = jwtIdentityRf.RefreshToken;
            var options = JwtTokenProducer.InitializeOptions(configuration);
            return JwtTokenProducer.Produce(jwtIdentity, options);
        }
    }
}
