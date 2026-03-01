using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNexus.Migrations
{
    /// <inheritdoc />
    public partial class removePurposeFromToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Tokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "Tokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
