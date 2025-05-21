using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_2025_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlsToPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl1",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl2",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl1",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "ImageUrl2",
                table: "Plans");
        }
    }
}
