using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AP.Repository.Migrations.General
{
    public partial class genAutobrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Link = table.Column<string>(nullable: true),
                    Parent = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryData",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Fullname = table.Column<string>(maxLength: 30, nullable: true),
                    Iso3Name = table.Column<string>(maxLength: 3, nullable: true),
                    PhoneCode = table.Column<int>(nullable: false),
                    Shortname = table.Column<string>(maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AutoBrand",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutoClassification = table.Column<int>(nullable: false),
                    Brand = table.Column<string>(maxLength: 50, nullable: true),
                    CountryID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoBrand", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AutoBrand_CountryData_CountryID",
                        column: x => x.CountryID,
                        principalTable: "CountryData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoBrand_CountryID",
                table: "AutoBrand",
                column: "CountryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoBrand");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "CountryData");
        }
    }
}
