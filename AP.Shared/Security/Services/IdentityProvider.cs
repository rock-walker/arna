using AP.Shared.Security.Contracts;
using AP.Core.Model;
using AP.Core.Model.User;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace AP.Shared.Security.Services
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IIdentityService identityService;
        private readonly ILogger<IdentityProvider> logger;

        public IdentityProvider(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IIdentityService service,
            ILogger<IdentityProvider> logger)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.identityService = service;
        }

        public async Task<JwtIdentity> OauthSignIn(LoginInfo info)
        {
            var user = await userManager.FindByEmailAsync(info.User);
            if (user == null)
            {
                logger.LogWarning($"User {info.User} doesn't exist.");
                return new JwtIdentity
                {
                    LoggedInStatus = IdentityStatus.Error
                };
            }

            var roles = await userManager.GetRolesAsync(user);

            var result = await signInManager.CheckPasswordSignInAsync(user, info.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                logger.LogInformation($"User {info.User} is logged in successfully.");
                return new JwtIdentity
                {
                    User = user,
                    LoggedInStatus = IdentityStatus.LoggedInSuccess,
                    Roles = roles
                };
            }
            if (result.IsLockedOut)
            {
                logger.LogWarning($"User {info.User} account locked out.");
                return new JwtIdentity
                {
                    LoggedInStatus = IdentityStatus.LockedOutPassordError
                };
            }

            return new JwtIdentity
            {
                LoggedInStatus = IdentityStatus.Error
            };
        }

        public async Task<IdentityStatus> SignIn(LoginInfo info)
        {
            var user = await userManager.FindByEmailAsync(info.User);
            if (user == null)
            {
                logger.LogWarning($"User {info.User} doesn't exist.");
                return IdentityStatus.Error;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, info.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                logger.LogInformation($"User {info.User} is logged in successfully.");
                return IdentityStatus.LoggedInSuccess;
            }

            if (result.RequiresTwoFactor)
            {
                return IdentityStatus.TwoFactorRequiresError;
                //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }

            return IdentityStatus.Error;
        }

        public RefreshToken GenerateRefreshToken(JwtIdentity identity)
        {
            var now = DateTime.UtcNow;
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                IssuedUtc = now,
            };
            refreshToken.ExpiresUtc = now.AddDays(300);
            identityService.PersistRefreshToken(refreshToken, identity.User);

            return refreshToken;
        }

        public void RevokeAccessToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<JwtIdentity> GetRefreshToken(string verifyRefreshToken)
        {
            var jwtIdentity = identityService.GetRefreshToken(verifyRefreshToken);
            if (jwtIdentity == null)
            {
                //add fallback here
                logger.LogWarning($"Refresh token doesn't exist");
                throw new Exception($"Refresh token was not found. User must relogin");
            }

            if (jwtIdentity.RefreshToken.Revoked)
            {
                logger.LogError($"Refresh token was revoked for user {jwtIdentity.User.UserName}");
                throw new Exception("Refresh token was revoked");
            }

            if (jwtIdentity.RefreshToken.ExpiresUtc < DateTime.UtcNow)
            {
                logger.LogWarning($"Refresh token was expired for user {jwtIdentity.User.UserName}. You have to relogin");
                throw new Exception("Refresh token was expired. You have to relogin");
            }

            if (!await signInManager.CanSignInAsync(jwtIdentity.User))
            {
                logger.LogError($"User {jwtIdentity.User.UserName} unable to Sign In. Was removed");
                throw new Exception("User unable to sign In. Reason: acount has been removed.");
            }

            if (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(jwtIdentity.User))
            {
                logger.LogError($"User {jwtIdentity.User.UserName} is locked out.");
                throw new Exception("User is locked out.");
            }

            jwtIdentity.RefreshToken = GenerateRefreshToken(jwtIdentity);

            jwtIdentity.Roles = await userManager.GetRolesAsync(jwtIdentity.User);
            jwtIdentity.Claims = await userManager.GetClaimsAsync(jwtIdentity.User);

            return jwtIdentity;
        }

        public async Task<JwtIdentity> ProvideOauthWorkflow(LoginInfo info)
        {
            var identity = await OauthSignIn(info);
            if (identity.LoggedInStatus == IdentityStatus.LoggedInSuccess)
            {
                var token = GenerateRefreshToken(identity);
                identity.RefreshToken = token;
                return identity;
            }

            return null;
        }
    }
}
