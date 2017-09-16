using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AP.Repository.Migrations
{
    public partial class AnchorAddedworkshop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnchorNumber",
                table: "Workshop",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "AvgRate",
                table: "Workshop",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "PayHour",
                table: "Workshop",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnchorNumber",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "AvgRate",
                table: "Workshop");

            migrationBuilder.DropColumn(
                name: "PayHour",
                table: "Workshop");
        }
    }
}
