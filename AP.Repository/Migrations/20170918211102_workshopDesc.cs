using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class workshopDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopID",
                table: "WorkshopCategories");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCategories_WorkshopID",
                table: "WorkshopCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkshopDataID",
                table: "WorkshopCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Workshop",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Unp",
                table: "Workshop",
                nullable: false,
                defaultValue: 0);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_WorkshopCategories_WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.DropColumn(
                name: "WorkshopDataID",
                table: "WorkshopCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "Unp",
                table: "Workshop");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopCategories_WorkshopID",
                table: "WorkshopCategories",
                column: "WorkshopID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopCategories_Workshop_WorkshopID",
                table: "WorkshopCategories",
                column: "WorkshopID",
                principalTable: "Workshop",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
