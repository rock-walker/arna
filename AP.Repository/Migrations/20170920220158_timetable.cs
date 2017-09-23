using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class timetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop");

            migrationBuilder.DropIndex(
                name: "IX_Workshop_AutoBrandID",
                table: "Workshop");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCategories_WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.DropColumn(
                name: "GoogleCode",
                table: "Address.City");

            migrationBuilder.DropColumn(
                name: "AutoBrandID",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.AlterColumn<decimal>(
                name: "PayHour",
                table: "Workshop",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateTable(
                name: "WorkshopWeekTimetable",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    DinnerEnd = table.Column<TimeSpan>(nullable: true),
                    DinnerStart = table.Column<TimeSpan>(nullable: true),
                    End = table.Column<TimeSpan>(nullable: false),
                    IsHoliday = table.Column<bool>(nullable: false),
                    Start = table.Column<TimeSpan>(nullable: false),
                    WorkshopID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopWeekTimetable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkshopWeekTimetable_Workshop_WorkshopID",
                        column: x => x.WorkshopID,
                        principalTable: "Workshop",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCategories_WorkshopID",
                table: "WorkshopCategories",
                column: "WorkshopID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopWeekTimetable_WorkshopID",
                table: "WorkshopWeekTimetable",
                column: "WorkshopID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopID",
                table: "WorkshopCategories",
                column: "WorkshopID",
                principalTable: "Workshop",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopID",
                table: "WorkshopCategories");

            migrationBuilder.DropTable(
                name: "WorkshopWeekTimetable");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCategories_WorkshopID",
                table: "WorkshopCategories");

            migrationBuilder.AddColumn<string>(
                name: "GoogleCode",
                table: "Address.City",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PayHour",
                table: "Workshop",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<int>(
                name: "AutoBrandID",
                table: "Workshop",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkshopDataID",
                table: "WorkshopCategories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workshop_AutoBrandID",
                table: "Workshop",
                column: "AutoBrandID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCategories_WorkshopDataID",
                table: "WorkshopCategories",
                column: "WorkshopDataID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopDataID",
                table: "WorkshopCategories",
                column: "WorkshopDataID",
                principalTable: "Workshop",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop",
                column: "AutoBrandID",
                principalTable: "AutoBrand",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
