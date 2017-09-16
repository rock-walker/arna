using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class country : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Address");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryID",
                table: "Address",
                nullable: false,
                defaultValue: new Guid());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Address",
                nullable: true);
        }
    }
}
