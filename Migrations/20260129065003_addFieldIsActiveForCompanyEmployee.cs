using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNexus.Migrations
{
    /// <inheritdoc />
    public partial class addFieldIsActiveForCompanyEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyEmployees",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyEmployees");
        }
    }
}
