using AP.EntityModel.AutoDomain;
using AP.EntityModel.Common;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using static AP.EntityModel.Common.DomainModels;

namespace AP.Repository.Context
{
    public class WorkshopContext : DbContext, IDbContext
    {
        public WorkshopContext(DbContextOptions<WorkshopContext> options) : base(options) { }

        protected DbSet<WorkshopIdentityUserDuplicate> Users { get; set; }
        public DbSet<WorkshopData> Workshops { get; set; }
        public DbSet<ContactData> Contacts { get; set; }
        public DbSet<GeoMarker> Locations { get; set; }
        public DbSet<AddressData> Addresses { get; set;}
        public DbSet<AvatarImage> Avatars { get; set; }
        //public DbSet<CategoryData> Categories { get; set; }
        public DbSet<WorkshopCategoryData> WorkshopCategory { get; set; }
        public DbSet<CityData> Cities { get; set; }
        public DbSet<CountryData> Countries { get; set; }
        public DbSet<WorkshopAutoBrand> WorkshopAutoBrands { get; set; }
        public DbSet<WorkshopDayTimetableData> WorkshopWeekTimetable { get; set; }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
        */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);

            //That is taken from another context
            builder.Entity<WorkshopIdentityUserDuplicate>().ToTable("Users");

            builder.Entity<WorkshopData>().ToTable("Workshop");
            //builder.Entity<CategoryData>().ToTable("Categories");
            builder.Entity<ContactData>().ToTable("Contacts");
            builder.Entity<GeoMarker>().ToTable("Markers");
            builder.Entity<AddressData>().ToTable("Address");
            builder.Entity<CityData>().ToTable("Address.City");
            builder.Entity<AvatarImage>().ToTable("Avatars");
            builder.Entity<CountryData>().ToTable("Countries");
            builder.Entity<WorkshopCategoryData>().ToTable("WorkshopCategories");
            builder.Entity<WorkshopAutoBrand>().ToTable("WorkshopAutobrands");
            builder.Entity<WorkshopDayTimetableData>().ToTable("WorkshopWeekTimetable");
        }
    }
}
