using AP.Core.Model.User;
using AP.Core.User.Authentication;
using AP.Repository.Context;
using AP.Shared.Security.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AP.Server
{
    public partial class Startup
    {
        public SymmetricSecurityKey signingKey;
        private IIdentityProvider identityProvider;

        private void ConfigureAuth(IApplicationBuilder app, IHostingEnvironment env)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));

            var tokenProviderOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            };

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name
            };

            var jwtOptions = new JwtBearerOptions
            {
                //Authority = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                RequireHttpsMetadata = true,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            };

            if (env.IsDevelopment())
            {
                jwtOptions.RequireHttpsMetadata = false;
            }

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            app.UseJwtBearerAuthentication(jwtOptions);

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(tokenProviderOptions));
            identityProvider = app.ApplicationServices.GetService<IIdentityProvider>();
        }

        private async Task<JwtUser> GetIdentity(LoginInfo info)
        {
            var loginStatus = await identityProvider.Login(info);
            if (loginStatus.Item2 == Core.Model.IdentityStatus.LoggedInSuccess)
            {
                return new JwtUser
                {
                    User = loginStatus.Item1,
                    Roles = loginStatus.Item3
                };
            }

            return null;
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            //Implemented only in 2.0 version
            //services.AddIdentityCore<>

            services.AddIdentity<ApplicationUser, ApplicationRole>(configuration =>
            {
                configuration.SignIn.RequireConfirmedEmail = false;
                configuration.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddEntityFrameworkStores<IdentityContext, Guid>();

            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
#if DEBUG
#else
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
#endif
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
