using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using AP.Repository.Context;

namespace AP.Repository.Migrations
{
    [DbContext(typeof(WorkshopContext))]
    partial class WorkshopContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AP.Core.Model.User.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AP.Core.Model.User.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.AutoBrand", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand");

                    b.Property<string>("Country");

                    b.HasKey("ID");

                    b.ToTable("AutoBrand");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.Workshop", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressID");

                    b.Property<int>("AutoBrandID");

                    b.Property<string>("BrandName");

                    b.Property<Guid>("ContactID");

                    b.Property<Guid>("LocationID");

                    b.Property<Guid?>("LogoID");

                    b.Property<string>("Name");

                    b.Property<string>("ShortName");

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("AutoBrandID");

                    b.HasIndex("ContactID");

                    b.HasIndex("LocationID");

                    b.HasIndex("LogoID");

                    b.ToTable("Workshop");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.WorkshopCategory", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryID");

                    b.Property<short>("MomentBookingState");

                    b.Property<Guid>("WorkshopID");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("WorkshopID");

                    b.ToTable("WorkshopCategories");
                });

            modelBuilder.Entity("AP.EntityModel.Common.Address", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Apartment");

                    b.Property<string>("Building");

                    b.Property<Guid>("CityID");

                    b.Property<string>("Comments");

                    b.Property<string>("Country");

                    b.Property<string>("Street");

                    b.Property<string>("SubApt");

                    b.Property<string>("SubBuilding");

                    b.HasKey("ID");

                    b.HasIndex("CityID");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("AP.EntityModel.Common.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Link");

                    b.Property<int>("Parent");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("AP.EntityModel.Common.City", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GoogleCode");

                    b.Property<string>("Name");

                    b.Property<string>("Ru");

                    b.HasKey("ID");

                    b.ToTable("Address.City");
                });

            modelBuilder.Entity("AP.EntityModel.Common.Contact", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Chat");

                    b.Property<string>("Email");

                    b.Property<string>("Mobile");

                    b.Property<string>("Municipal");

                    b.Property<string>("Web");

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("AP.EntityModel.Common.DomainModels+AvatarImage", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Image");

                    b.HasKey("ID");

                    b.ToTable("Avatars");
                });

            modelBuilder.Entity("AP.EntityModel.Common.GeoMarker", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Lat");

                    b.Property<double>("Lng");

                    b.HasKey("ID");

                    b.ToTable("Markers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.Workshop", b =>
                {
                    b.HasOne("AP.EntityModel.Common.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.AutoDomain.AutoBrand", "AutoBrand")
                        .WithMany()
                        .HasForeignKey("AutoBrandID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.Common.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.Common.GeoMarker", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.Common.DomainModels+AvatarImage", "Logo")
                        .WithMany()
                        .HasForeignKey("LogoID");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.WorkshopCategory", b =>
                {
                    b.HasOne("AP.EntityModel.Common.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.AutoDomain.Workshop", "WorkshopData")
                        .WithMany("WorkshopCategories")
                        .HasForeignKey("WorkshopID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AP.EntityModel.Common.Address", b =>
                {
                    b.HasOne("AP.EntityModel.Common.City", "City")
                        .WithMany()
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("AP.Core.Model.User.ApplicationRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("AP.Core.Model.User.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("AP.Core.Model.User.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("AP.Core.Model.User.ApplicationRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.Core.Model.User.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
