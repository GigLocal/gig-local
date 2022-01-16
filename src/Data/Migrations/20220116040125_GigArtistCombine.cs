using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigLocal.Data.Migrations
{
    public partial class GigArtistCombine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "Gig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Gig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventUrl",
                table: "Gig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Gig",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "Gig");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Gig");

            migrationBuilder.DropColumn(
                name: "EventUrl",
                table: "Gig");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Gig");
        }
    }
}
