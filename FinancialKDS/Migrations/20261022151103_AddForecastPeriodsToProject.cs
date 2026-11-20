using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialKDS.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastPeriodsToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForecastPeriods",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForecastPeriods",
                table: "Projects");
        }
    }
}
