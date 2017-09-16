using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class workshopautobrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_Address_AddressID",
                table: "Workshop");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_Contacts_ContactID",
                table: "Workshop");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactID",
                table: "Workshop",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "AutoBrandID",
                table: "Workshop",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressID",
                table: "Workshop",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "WorkshopAutobrands",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    AutoBrandID = table.Column<int>(nullable: false),
                    WorkshopID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopAutobrands", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkshopAutobrands_AutoBrand_AutoBrandID",
                        column: x => x.AutoBrandID,
                        principalTable: "AutoBrand",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkshopAutobrands_Workshop_WorkshopID",
                        column: x => x.WorkshopID,
                        principalTable: "Workshop",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopAutobrands_AutoBrandID",
                table: "WorkshopAutobrands",
                column: "AutoBrandID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopAutobrands_WorkshopID",
                table: "WorkshopAutobrands",
                column: "WorkshopID");

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_Address_AddressID",
                table: "Workshop",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop",
                column: "AutoBrandID",
                principalTable: "AutoBrand",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_Contacts_ContactID",
                table: "Workshop",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_Address_AddressID",
                table: "Workshop");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop");

            migrationBuilder.DropForeignKey(
                name: "FK_Workshop_Contacts_ContactID",
                table: "Workshop");

            migrationBuilder.DropTable(
                name: "WorkshopAutobrands");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactID",
                table: "Workshop",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AutoBrandID",
                table: "Workshop",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressID",
                table: "Workshop",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_Address_AddressID",
                table: "Workshop",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_AutoBrand_AutoBrandID",
                table: "Workshop",
                column: "AutoBrandID",
                principalTable: "AutoBrand",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshop_Contacts_ContactID",
                table: "Workshop",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
