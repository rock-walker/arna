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
using System.Security.Claims;

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
            var result = await userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                logger.LogInformation($"Role {roleName} added to user {user.UserName} successfully.");
            }
            else
            {
                logger.LogError($"Failed to add role {roleName} to user {user.UserName}.");
            }
        }

        public async Task AddClaim(ApplicationUser user, ApplicationClaims appClaim)
        {
            var claimName = Enum.GetName(typeof(ApplicationClaims), appClaim);
            var result = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.AuthorizationDecision, claimName));
            if (result.Succeeded)
            {
                logger.LogInformation($"Claim {claimName} added to User {user.UserName} successfully.");
            }
        }

        public async Task<ApplicationUser> FindById(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> FindByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<JwtResponse> CompleteUserVerification(ApplicationUser user, string phoneNumber, string code)
        {
            var result = await userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            if (result.Succeeded)
            {
                //CHECK: looks like I don't need it anymore (most likely uses cookie)
                //await signInManager.SignInAsync(user, isPersistent: false);

                //TODO: rethink this approach
                await AddClaim(user, ApplicationClaims.Verified);
                var jwtIdentity = await CreateJwt(user);

                var refreshToken = identityProvider.GenerateRefreshToken(jwtIdentity);
                jwtIdentity.RefreshToken = refreshToken;

                var options = JwtTokenProducer.InitializeOptions(configuration);
                return JwtTokenProducer.Produce(jwtIdentity, options);
            }

            logger.LogError($"Failed to complete user {user.UserName} verification with ex: {result.Errors.First().Description}.");
            return null;
        }

        public async Task<JwtResponse> RefreshJwt(ApplicationUser user)
        {
            var jwtIdentity = await CreateJwt(user);
            var refreshToken = identityProvider.GenerateRefreshToken(jwtIdentity);

            jwtIdentity.RefreshToken = refreshToken;
            var options = JwtTokenProducer.InitializeOptions(configuration);
            return JwtTokenProducer.Produce(jwtIdentity, options);
        }

        private async Task<JwtIdentity> CreateJwt(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = await userManager.GetClaimsAsync(user);

            return new JwtIdentity
            {
                User = user,
                LoggedInStatus = IdentityStatus.LoggedInSuccess,
                Roles = roles,
                Claims = claims
            };
        }
    }
}
