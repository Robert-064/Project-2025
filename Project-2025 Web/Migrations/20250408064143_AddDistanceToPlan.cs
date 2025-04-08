using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_2025_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDistanceToPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Plans",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Plans");
        }
    }
}
