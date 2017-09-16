using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class country11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Address.City_CityID",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "Address");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryID",
                table: "Address.City",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CityID",
                table: "Address",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryID",
                table: "AutoBrand",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_Address.City_CountryID",
                table: "Address.City",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_AutoBrand_CountryID",
                table: "AutoBrand",
                column: "CountryID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoBrand_Countries_CountryID",
                table: "AutoBrand",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Address.City_CityID",
                table: "Address",
                column: "CityID",
                principalTable: "Address.City",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address.City_Countries_CountryID",
                table: "Address.City",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoBrand_Countries_CountryID",
                table: "AutoBrand");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Address.City_CityID",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address.City_Countries_CountryID",
                table: "Address.City");

            migrationBuilder.DropIndex(
                name: "IX_Address.City_CountryID",
                table: "Address.City");

            migrationBuilder.DropIndex(
                name: "IX_AutoBrand_CountryID",
                table: "AutoBrand");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "Address.City");

            migrationBuilder.AlterColumn<Guid>(
                name: "CityID",
                table: "Address",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryID",
                table: "Address",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryID",
                table: "AutoBrand",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Address.City_CityID",
                table: "Address",
                column: "CityID",
                principalTable: "Address.City",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
