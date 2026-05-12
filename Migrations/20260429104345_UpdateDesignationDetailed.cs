using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDesignationDetailed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuelLimitPerMonth",
                table: "Designations");

            migrationBuilder.AlterColumn<string>(
                name: "DesignationType",
                table: "Designations",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Designations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselFuelLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselMaintenanceLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolFuelLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolMaintenanceLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_DepartmentId",
                table: "Designations",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Departments_DepartmentId",
                table: "Designations",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Departments_DepartmentId",
                table: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Designations_DepartmentId",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "DieselFuelLimit",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "DieselMaintenanceLimit",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "PetrolFuelLimit",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "PetrolMaintenanceLimit",
                table: "Designations");

            migrationBuilder.AlterColumn<int>(
                name: "DesignationType",
                table: "Designations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "FuelLimitPerMonth",
                table: "Designations",
                type: "numeric",
                nullable: true);
        }
    }
}
