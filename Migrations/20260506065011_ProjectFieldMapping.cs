using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ProjectFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Departments_DepartmentId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PetrolFuelLimit",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PetrolMaintenanceLimit",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Projects",
                newName: "DeptId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Projects",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_DepartmentId",
                table: "Projects",
                newName: "IX_Projects_DeptId");

            migrationBuilder.AddColumn<double>(
                name: "FuelLitresPerMonth",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MaintenanceAmtPerAnnum",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MaintenanceAmtPerMonth",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                columns: new[] { "FuelLitresPerMonth", "MaintenanceAmtPerAnnum", "MaintenanceAmtPerMonth", "PDate", "ProjectName", "TDate" },
                values: new object[] { 1000.0, 600000.0, 50000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State Road Maintenance", null });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                columns: new[] { "FuelLitresPerMonth", "MaintenanceAmtPerAnnum", "MaintenanceAmtPerMonth", "PDate", "ProjectName", "TDate" },
                values: new object[] { 1500.0, 900000.0, 75000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NHAI Punjab Segment", null });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 3,
                columns: new[] { "FuelLitresPerMonth", "MaintenanceAmtPerAnnum", "MaintenanceAmtPerMonth", "PDate", "ProjectName", "TDate" },
                values: new object[] { 800.0, 480000.0, 40000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smart City Ludhiana", null });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 4,
                columns: new[] { "FuelLitresPerMonth", "MaintenanceAmtPerAnnum", "MaintenanceAmtPerMonth", "PDate", "ProjectName", "TDate" },
                values: new object[] { 600.0, 360000.0, 30000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Border Area Development", null });

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Departments_DeptId",
                table: "Projects",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Departments_DeptId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FuelLitresPerMonth",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MaintenanceAmtPerAnnum",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MaintenanceAmtPerMonth",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                table: "Projects",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Projects",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_DeptId",
                table: "Projects",
                newName: "IX_Projects_DepartmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolFuelLimit",
                table: "Projects",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PetrolMaintenanceLimit",
                table: "Projects",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "IsActive", "Name", "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "State Road Maintenance", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsActive", "Name", "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "NHAI Punjab Segment", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "IsActive", "Name", "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Smart City Ludhiana", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "IsActive", "Name", "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Border Area Development", 0m, 0m });

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Departments_DepartmentId",
                table: "Projects",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
