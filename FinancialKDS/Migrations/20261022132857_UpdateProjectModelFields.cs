using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialKDS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectModelFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisResults_ProjectId",
                table: "AnalysisResults");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "Projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ForecastPeriod",
                table: "Projects",
                newName: "PeriodsToForecast");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Projects",
                newName: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_ProjectId",
                table: "AnalysisResults",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisResults_ProjectId",
                table: "AnalysisResults");

            migrationBuilder.RenameColumn(
                name: "PeriodsToForecast",
                table: "Projects",
                newName: "ForecastPeriod");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Projects",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Projects",
                newName: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_ProjectId",
                table: "AnalysisResults",
                column: "ProjectId",
                unique: true);
        }
    }
}
