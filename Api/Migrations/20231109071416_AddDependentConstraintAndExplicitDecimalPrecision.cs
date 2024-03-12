using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDependentConstraintAndExplicitDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dependents_EmployeeId",
                table: "Dependents");

            migrationBuilder.AddColumn<bool>(
                name: "IsPartner",
                table: "Dependents",
                type: "bit",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN Relationship = 1 OR Relationship = 2 THEN 1 ELSE 0 END AS BIT)");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_EmployeeId_IsPartner",
                table: "Dependents",
                columns: new[] { "EmployeeId", "IsPartner" },
                unique: true,
                filter: "Relationship IN (1, 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dependents_EmployeeId_IsPartner",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "IsPartner",
                table: "Dependents");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_EmployeeId",
                table: "Dependents",
                column: "EmployeeId");
        }
    }
}
