using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingNormalizePlanName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedPlanName",
                table: "PaymentPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedFeeTypeName",
                table: "FeeTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedPlanName",
                table: "PaymentPlans");

            migrationBuilder.DropColumn(
                name: "NormalizedFeeTypeName",
                table: "FeeTypes");
        }
    }
}
