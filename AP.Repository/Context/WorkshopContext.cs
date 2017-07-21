using AP.EntityModel.AutoDomain;
using AP.EntityModel.Common;
using Microsoft.EntityFrameworkCore;
using static AP.EntityModel.Common.DomainModels;

namespace AP.Repository.Context
{
    public class WorkshopContext : DbContext
    {
        public WorkshopContext(DbContextOptions<WorkshopContext> options) : base(options) { }

        public DbSet<EntityModel.AutoDomain.Workshop> Workshops { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<GeoMarker> Locations { get; set; }
        public DbSet<Address> Addresses { get; set;}
        public DbSet<AvatarImage> Avatars { get; set; }
        public DbSet<AutoBrand> Autobrands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WorkshopCategory> WorkshopCategory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EntityModel.AutoDomain.Workshop>().ToTable("Workshop");
            builder.Entity<Category>().ToTable("Categories");
            builder.Entity<Contact>().ToTable("Contacts");
            builder.Entity<GeoMarker>().ToTable("Markers");
            builder.Entity<Address>().ToTable("Address");
            builder.Entity<City>().ToTable("Address.City");
            builder.Entity<AvatarImage>().ToTable("Avatars");
            builder.Entity<AutoBrand>().ToTable("AutoBrand");
            builder.Entity<WorkshopCategory>().ToTable("WorkshopCategories");
        }
    }
}
