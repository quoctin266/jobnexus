using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNexus.Migrations
{
    /// <inheritdoc />
    public partial class modifyJobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jobs");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
