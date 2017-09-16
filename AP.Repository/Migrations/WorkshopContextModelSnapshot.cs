using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AP.Repository.Context;
using AP.Business.Model.Enums;

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

            modelBuilder.Entity("AP.EntityModel.AutoDomain.AutoBrand", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand")
                        .HasMaxLength(50);

                    b.Property<Guid>("CountryID");

                    b.HasKey("ID");

                    b.HasIndex("CountryID");

                    b.ToTable("AutoBrand");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.Workshop", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AddressID");

                    b.Property<int>("AnchorNumber");

                    b.Property<int?>("AutoBrandID");

                    b.Property<float>("AvgRate");

                    b.Property<string>("BrandName")
                        .HasMaxLength(50);

                    b.Property<Guid?>("ContactID");

                    b.Property<Guid>("LocationID");

                    b.Property<Guid?>("LogoID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.Property<decimal>("PayHour");

                    b.Property<string>("ShortName")
                        .HasMaxLength(20);

                    b.HasKey("ID");

                    b.HasIndex("AddressID");

                    b.HasIndex("AutoBrandID");

                    b.HasIndex("ContactID");

                    b.HasIndex("LocationID");

                    b.HasIndex("LogoID");

                    b.ToTable("Workshop");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.WorkshopAutoBrand", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AutoBrandID");

                    b.Property<Guid>("WorkshopID");

                    b.HasKey("ID");

                    b.HasIndex("AutoBrandID");

                    b.HasIndex("WorkshopID");

                    b.ToTable("WorkshopAutobrands");
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

                    b.Property<string>("Building")
                        .HasMaxLength(20);

                    b.Property<Guid?>("CityID");

                    b.Property<string>("Comments")
                        .HasMaxLength(256);

                    b.Property<string>("Street")
                        .HasMaxLength(50);

                    b.Property<string>("SubApt")
                        .HasMaxLength(10);

                    b.Property<string>("SubBuilding")
                        .HasMaxLength(10);

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

                    b.Property<Guid?>("CountryID");

                    b.Property<string>("GoogleCode");

                    b.Property<string>("Name")
                        .HasMaxLength(30);

                    b.Property<string>("Ru")
                        .HasMaxLength(30);

                    b.HasKey("ID");

                    b.HasIndex("CountryID");

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

            modelBuilder.Entity("AP.EntityModel.Common.Country", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Fullname")
                        .HasMaxLength(30);

                    b.Property<string>("Iso3Name")
                        .HasMaxLength(3);

                    b.Property<int>("PhoneCode");

                    b.Property<string>("Shortname")
                        .HasMaxLength(2);

                    b.HasKey("ID");

                    b.ToTable("Countries");
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

            modelBuilder.Entity("AP.EntityModel.AutoDomain.AutoBrand", b =>
                {
                    b.HasOne("AP.EntityModel.Common.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.Workshop", b =>
                {
                    b.HasOne("AP.EntityModel.Common.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID");

                    b.HasOne("AP.EntityModel.AutoDomain.AutoBrand", "AutoBrand")
                        .WithMany()
                        .HasForeignKey("AutoBrandID");

                    b.HasOne("AP.EntityModel.Common.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactID");

                    b.HasOne("AP.EntityModel.Common.GeoMarker", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.Common.DomainModels+AvatarImage", "Logo")
                        .WithMany()
                        .HasForeignKey("LogoID");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.WorkshopAutoBrand", b =>
                {
                    b.HasOne("AP.EntityModel.AutoDomain.AutoBrand", "AutoBrand")
                        .WithMany()
                        .HasForeignKey("AutoBrandID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AP.EntityModel.AutoDomain.Workshop", "Workshop")
                        .WithMany("AutoBrands")
                        .HasForeignKey("WorkshopID")
                        .OnDelete(DeleteBehavior.Cascade);
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
                        .HasForeignKey("CityID");
                });

            modelBuilder.Entity("AP.EntityModel.Common.City", b =>
                {
                    b.HasOne("AP.EntityModel.Common.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryID");
                });
        }
    }
}
