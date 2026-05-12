using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DesignationFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Departments_DepartmentId",
                table: "Designations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Designations",
                newName: "DesignationName");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Designations",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Designations",
                newName: "DeptId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Designations",
                newName: "DesignationId");

            migrationBuilder.RenameIndex(
                name: "IX_Designations_DepartmentId",
                table: "Designations",
                newName: "IX_Designations_DeptId");

            migrationBuilder.AlterColumn<int>(
                name: "PetrolMaintenanceLimit",
                table: "Designations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "PetrolFuelLimit",
                table: "Designations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "DieselMaintenanceLimit",
                table: "Designations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "DieselFuelLimit",
                table: "Designations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Designations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Designations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Departments_DeptId",
                table: "Designations",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designations_Departments_DeptId",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Designations");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Designations",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "DesignationName",
                table: "Designations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                table: "Designations",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "DesignationId",
                table: "Designations",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Designations_DeptId",
                table: "Designations",
                newName: "IX_Designations_DepartmentId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PetrolMaintenanceLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "PetrolFuelLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "DieselMaintenanceLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "DieselFuelLimit",
                table: "Designations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Designations_Departments_DepartmentId",
                table: "Designations",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
