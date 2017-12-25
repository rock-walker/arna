using System;
#if EF6
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
#endif
using System.Threading;
using System.Threading.Tasks;
#if NETCOREAPP1_1
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
#elif EF6
#endif

namespace EntityFramework.DbContextScope.Interfaces
{
#if NETCOREAPP1_1
    public interface IDbContext : IDisposable
#elif EF6
    public interface IDbContext : IDisposable, IObjectContextAdapter
#endif
    {
#if NETCOREAPP1_1
        ChangeTracker ChangeTracker { get; }
        DatabaseFacade Database { get; }
#elif EF6
        DbContextConfiguration Configuration { get; }
        Database Database { get; }
#endif

        int SaveChanges();
#if !NET40
        Task<int> SaveChangesAsync(CancellationToken cancelToken);
#endif
    }
}