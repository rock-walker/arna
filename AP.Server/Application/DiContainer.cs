using System;
using AP.Repository.Context;
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
using AP.Repository.Common.Contracts;
using AP.Repository.Common.Services;
using AP.Business.Domain.Common;
using AP.EntityModel.ReadModel;
using AP.Business.Registration.ReadModel;
using AP.Business.Registration.ReadModel.Implementation;
using AP.Infrastructure.Sql.EventSourcing;
using AP.Infrastructure.EventSourcing;
using Infrastructure.Sql.EventSourcing;
using AP.Infrastructure.Sql.MessageLog;
using AP.Business.Registration.Database;
using AP.Infrastructure.Processes;
using AP.Business.Registration;
using AP.Infrastructure.Sql.Processes;
using AP.Infrastructure.Messaging;
using AP.Infrastructure.Serialization;
using Microsoft.Extensions.Logging;
using AP.Business.Attendee;
using AP.Repository.Attendee.Contracts;
using AP.Repository.Attendee.Services;
using AP.Repository.Booking.Contracts;
using AP.Repository.Booking.Services;
using AP.Business.Domain.Common.Category;

namespace AP.Server.Application
{
    public class DiContainer
    {
        private static string _dbConnectionName = "AutoPortalConnection";

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

            BookingContainer.SetupBookingHandlers(services);
            BookingContainer.OnCreateContainer(services);
        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IWorkshopService, WorkshopService>();
            services.AddScoped<IWorkshopAccountService, WorkshopAccountService>();
            services.AddScoped<IWorkshopFilterService, WorkshopFilterService>();
            services.AddScoped<IEmailSender, AuthEmailSenderService>();
            services.AddScoped<ISmsSender, TwilioSmsSenderService>();
            services.AddScoped<IAutobrandService, AutobrandService>();
            services.AddScoped<IAttendeeAccountService, AttendeeAccountService>();
            services.AddScoped<IOrderDao, OrderDao>();
            services.AddSingleton<IWorkshopDao, CachingWorkshopDao>();
            services.AddScoped<IWorkshopDao, WorkshopDao>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IDbContextScopeFactory>(provider => 
                new DbContextScopeFactory(
                    new DbContextFactoryInjector(provider)));

            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IWorkshopRepository, WorkshopRepository>();
            services.AddScoped<IWorkshopAccountRepository, WorkshopAccountRepository>();
            services.AddScoped<IWorkshopFilterRepository, WorkshopFilterRepository>();
            services.AddScoped<IAutobrandRepository, AutobrandRepository>();
            services.AddScoped<IAttendeeAccountRepository, AttendeeAccountRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }

        private static void RegisterDbContexts(IServiceCollection services, IConfigurationRoot config)
        {
            var connectionString = config.GetConnectionString(_dbConnectionName);

            services.AddDbContext<GeneralContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            //CQRS proposes to make WorkshopContext transient
            services.AddDbContext<WorkshopContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<AttendeeContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddDbContext<WorkshopRegistrationDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddDbContext<RegistrationProcessManagerDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddTransient<Func<IProcessManagerDataContext<RegistrationProcessManager>>, Func<SqlProcessManagerDataContext<RegistrationProcessManager>>>(
                provider => () => new SqlProcessManagerDataContext<RegistrationProcessManager>(() => provider.GetService<RegistrationProcessManagerDbContext>(), 
                provider.GetService<ICommandBus>(), provider.GetService<ITextSerializer>(), provider.GetService<ILogger>()));
            services.AddDbContext<EventStoreDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddSingleton(typeof(IEventSourcedRepository<>), typeof(SqlEventSourcedRepository<>));
            services.AddDbContext<MessageLogDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

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
