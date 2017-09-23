using System;
using AP.Repository.Common;
using AP.Repository.Context;
using AP.Shared.Category;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AP.Repository.Workshop.Contracts;
using AP.Repository.Workshop.Services;
using AP.Business.AutoDomain.Workshop.Services;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Core.Model.User;
using Microsoft.AspNetCore.Builder;
using AP.Shared.Sender.Contracts;
using AP.Shared.Sender.Services;
using AP.Core.Model.Authentication;
using AP.Business.AutoPortal.Workshop.Contracts;
using AP.Business.AutoPortal.Workshop.Services;
using EntityFramework.DbContextScope.Interfaces;
using EntityFramework.DbContextScope;
using AP.Repository.Infrastructure;

namespace AP.Server.Application
{
    public class DiContainer
    {
        private static string _dbConnectionName = "AutoPortalConnection";
        private static string _connectionString;

        public static void RegisterScopes(IServiceCollection services, IConfigurationRoot configuration)
        {
            /*
            var appRoot = env.ContentRootPath;
            var root = Directory.GetParent(appRoot);
            var repositoryAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(root + "\\AP.Repository\\bin\\Debug\\netcoreapp1.1\\AP.Repository.dll");
            var repositoryType = repositoryAssembly.GetType("AP.Repository.Application.Contracts.IContainerRegister");
            var repositoryRegistrator = Activator.CreateInstance(repositoryType);
            */
            RegisterControllers(services);
            RegisterRepositories(services);
            RegisterDbContexts(services, configuration);

            RegisterAuthentication(services, configuration);
        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWorkshopService, WorkshopService>();
            services.AddScoped<IWorkshopBookingService, WorkshopBookingService>();
            services.AddScoped<IWorkshopAccountService, WorkshopAccountService>();
            services.AddScoped<IEmailSender, AuthEmailSenderService>();
            services.AddScoped<ISmsSender, TwilioSmsSenderService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IDbContextScopeFactory>(provider => 
                new DbContextScopeFactory(
                    new DbContextFactoryInjector(provider)));

            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWorkshopRepository, WorkshopRepository>();
            services.AddScoped<IWorkshopBookingRepository, WorkshopBookingRepository>();
            services.AddScoped<IWorkshopAccountRepository, WorkshopAccountRepository>();
        }

        private static void RegisterDbContexts(IServiceCollection services, IConfigurationRoot config)
        {
            var connectionString = config.GetConnectionString(_dbConnectionName);

            services.AddDbContext<GeneralContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<WorkshopContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>(configuration =>
                {
                    configuration.SignIn.RequireConfirmedEmail = false;
                    configuration.SignIn.RequireConfirmedPhoneNumber = true;
                })
                .AddEntityFrameworkStores<IdentityContext, Guid>()
                .AddDefaultTokenProviders();

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
                options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOut";

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }

        private static void RegisterAuthentication(IServiceCollection services, IConfiguration config)
        {
            services.Configure<AuthMessageSenderOptions>(config);
            services.Configure<TwilioSmsOptions>(config.GetSection("Twilio"));
        }
    }
}
