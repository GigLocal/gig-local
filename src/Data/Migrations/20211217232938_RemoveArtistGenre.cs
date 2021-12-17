using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigLocal.Data.Migrations
{
    public partial class RemoveArtistGenre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Artist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Artist",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
