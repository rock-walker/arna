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
        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWorkshopService, WorkshopService>();
            services.AddScoped<IWorkshopBookingService, WorkshopBookingService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWorkshopRepository, WorkshopRepository>();
            services.AddScoped<IWorkshopBookingRepository, WorkshopBookingRepository>();
        }

        private static void RegisterDbContexts(IServiceCollection services, IConfigurationRoot config)
        {
            var connectionString = config.GetConnectionString(_dbConnectionName);

            services.AddDbContext<GeneralContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<WorkshopContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
