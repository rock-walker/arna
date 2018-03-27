using AP.Core.Model.User;
using AP.Repository.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AP.Server.Application
{
    public class Authentication
    {
        public static void Configure(IServiceCollection services)
        {

            services.AddAuthentication(options =>
            {
                options.SignInScheme = JwtBearerDefaults.AuthenticationScheme
            });

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

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/api/Account/Login";
                options.Cookies.ApplicationCookie.LogoutPath = "/api/Account/Logout";

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
