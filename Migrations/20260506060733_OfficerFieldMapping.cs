using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class OfficerFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Offices_OfficeId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_OfficeId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DieselFuelLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DieselMaintenanceLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "PetrolFuelLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "PetrolMaintenanceLimit",
                table: "Officers");

            migrationBuilder.RenameColumn(
                name: "HRMSCode",
                table: "Officers",
                newName: "HrmsCode");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Officers",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Officers",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Officers",
                newName: "OfficerName");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Officers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DesignationId",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeptId",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DesignationType",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Officers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FuelLimit",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuelLimmitd",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceLimit",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceLimitd",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OfficerId",
                table: "Officers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Officers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DeptId",
                table: "Officers",
                column: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Departments_DeptId",
                table: "Officers",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Departments_DeptId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DeptId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DesignationType",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "FuelLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "FuelLimmitd",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "MaintenanceLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "MaintenanceLimitd",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "OfficerId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Officers");

            migrationBuilder.RenameColumn(
                name: "HrmsCode",
                table: "Officers",
                newName: "HRMSCode");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Officers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "OfficerName",
                table: "Officers",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Officers",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Officers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DesignationId",
                table: "Officers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "DieselFuelLimit",
                table: "Officers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselMaintenanceLimit",
                table: "Officers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OfficeId",
                table: "Officers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolFuelLimit",
                table: "Officers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolMaintenanceLimit",
                table: "Officers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Officers_OfficeId",
                table: "Officers",
                column: "OfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Offices_OfficeId",
                table: "Officers",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id");
        }
    }
}
