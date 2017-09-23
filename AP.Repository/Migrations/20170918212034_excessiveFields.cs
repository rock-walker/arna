using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class excessiveFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "Chat",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Municipal",
                table: "Contacts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "Workshop",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Workshop",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Chat",
                table: "Contacts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipal",
                table: "Contacts",
                nullable: true);
        }
    }
}
