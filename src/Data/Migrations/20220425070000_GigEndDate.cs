using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigLocal.Data.Migrations
{
    public partial class GigEndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Gig",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Gig",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Gig");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Gig",
                newName: "Date");
        }
    }
}
