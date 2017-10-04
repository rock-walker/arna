using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AP.Repository.Context;
using AP.Business.Model.Enums;

namespace AP.Repository.Migrations.General
{
    [DbContext(typeof(GeneralContext))]
    [Migration("20171002230742_genAutobrand")]
    partial class genAutobrand
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AP.EntityModel.AutoDomain.AutoBrandData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AutoClassification");

                    b.Property<string>("Brand")
                        .HasMaxLength(50);

                    b.Property<Guid>("CountryID");

                    b.HasKey("ID");

                    b.HasIndex("CountryID");

                    b.ToTable("AutoBrand");
                });

            modelBuilder.Entity("AP.EntityModel.Common.CategoryData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Link");

                    b.Property<int>("Parent");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("AP.EntityModel.Common.CountryData", b =>
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

                    b.ToTable("CountryData");
                });

            modelBuilder.Entity("AP.EntityModel.AutoDomain.AutoBrandData", b =>
                {
                    b.HasOne("AP.EntityModel.Common.CountryData", "Country")
                        .WithMany()
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
