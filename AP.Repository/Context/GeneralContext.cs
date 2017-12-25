using AP.EntityModel.AutoDomain;
using AP.EntityModel.Common;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AP.Repository.Context
{
    public class GeneralContext : DbContext, IDbContext
    {
        private const string _schema = "Info";
        public GeneralContext(DbContextOptions<GeneralContext> options) : base(options)
        {

        }
        public DbSet<CategoryData> Categories { get; set; }
        public DbSet<AutoBrandData> Autobrands { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryData>().ToTable("Categories", _schema);
            builder.Entity<AutoBrandData>().ToTable("AutoBrands", _schema);
        }
    }
}
