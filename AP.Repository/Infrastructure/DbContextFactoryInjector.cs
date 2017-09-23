using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFramework.DbContextScope.Interfaces;
using System;

namespace AP.Repository.Infrastructure
{
    public class DbContextFactoryInjector :IDbContextFactory
    {
        readonly IServiceProvider _container;

        public DbContextFactoryInjector(IServiceProvider container)
        {
            _container = container;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : class, IDbContext
        {
            return _container.GetService<TDbContext>();
        }
    }
}
