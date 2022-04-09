using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigLocal.Data.Migrations
{
    public partial class VenueImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Venue",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Venue");
        }
    }
}
