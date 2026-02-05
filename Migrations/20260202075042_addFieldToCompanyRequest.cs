using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNexus.Migrations
{
    /// <inheritdoc />
    public partial class addFieldToCompanyRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "CompanyRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "CompanyRequests");
        }
    }
}
