using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigLocal.Data.Migrations
{
    public partial class GigApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Gig",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Gig");
        }
    }
}
