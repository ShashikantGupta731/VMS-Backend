using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class VehicleInfoLegacyFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Departments_DepartmentId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Designations_DesignationId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Offices_OfficeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_DepartmentId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleCost",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "VerificationStatus",
                table: "Vehicles",
                newName: "verificationstatus");

            migrationBuilder.RenameColumn(
                name: "VehicleProofUploads",
                table: "Vehicles",
                newName: "vehicleproofuploads");

            migrationBuilder.RenameColumn(
                name: "VerificationComments",
                table: "Vehicles",
                newName: "VehicleNOC");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Vehicles",
                newName: "VehiclePurchaseDate");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Vehicles",
                newName: "RequisitionOfficeId");

            migrationBuilder.RenameColumn(
                name: "DDOCode",
                table: "Vehicles",
                newName: "NodalOfficerUsername");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Vehicles",
                newName: "VehicleInfoId");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DDOId",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeptId",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialYearReading",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FitnessUpto",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FuelConsumptionCostDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsTyreOriginal",
                table: "Vehicles",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Vehicles",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KM_30062017",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastThreeYearsMaintenanceCostDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTyreChangedDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastTyreChangedKM",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenenceDuration",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NOC_IssueDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NodalOfficerEmail",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NodalOfficerMobileNo",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NodalOfficerName",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadingUptodate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequisitionDeptId",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "temporpermanent",
                table: "Vehicles",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DeptId",
                table: "Vehicles",
                column: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Departments_DeptId",
                table: "Vehicles",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Designations_DesignationId",
                table: "Vehicles",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Offices_OfficeId",
                table: "Vehicles",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Departments_DeptId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Designations_DesignationId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Offices_OfficeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_DeptId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DDOId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FinancialYearReading",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FitnessUpto",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FuelConsumptionCostDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsTyreOriginal",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "KM_30062017",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastThreeYearsMaintenanceCostDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastTyreChangedDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastTyreChangedKM",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "MaintenenceDuration",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NOC_IssueDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NodalOfficerEmail",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NodalOfficerMobileNo",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NodalOfficerName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ReadingUptodate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RequisitionDeptId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "temporpermanent",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "verificationstatus",
                table: "Vehicles",
                newName: "VerificationStatus");

            migrationBuilder.RenameColumn(
                name: "vehicleproofuploads",
                table: "Vehicles",
                newName: "VehicleProofUploads");

            migrationBuilder.RenameColumn(
                name: "VehiclePurchaseDate",
                table: "Vehicles",
                newName: "PurchaseDate");

            migrationBuilder.RenameColumn(
                name: "VehicleNOC",
                table: "Vehicles",
                newName: "VerificationComments");

            migrationBuilder.RenameColumn(
                name: "RequisitionOfficeId",
                table: "Vehicles",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "NodalOfficerUsername",
                table: "Vehicles",
                newName: "DDOCode");

            migrationBuilder.RenameColumn(
                name: "VehicleInfoId",
                table: "Vehicles",
                newName: "Id");

            migrationBuilder.AddColumn<decimal>(
                name: "VehicleCost",
                table: "Vehicles",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DepartmentId",
                table: "Vehicles",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Departments_DepartmentId",
                table: "Vehicles",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Designations_DesignationId",
                table: "Vehicles",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Offices_OfficeId",
                table: "Vehicles",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
