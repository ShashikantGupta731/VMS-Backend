using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SyncAllLegacyModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_Departments_DeptId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_Districts_DistrictId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_VehicleTypes_VehicleTypeId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Departments_DeptId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Departments_DeptId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Offices_DeptId",
                table: "Offices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Officers",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DeptId",
                table: "Officers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleTypes",
                newName: "VehicleTypeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleTransfers",
                newName: "VehicleTransferId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleModels",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleCondemnations",
                newName: "VehicleCondemnationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tehsils",
                newName: "TehsilId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StockTransactions",
                newName: "StockTransactionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Roles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Offices",
                newName: "OfficeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Officers",
                newName: "DepartmentDeptId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Manufacturers",
                newName: "ManufacturerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MaintenanceBills",
                newName: "MaintenanceBillId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InventoryItems",
                newName: "InventoryItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InventoryAllotments",
                newName: "InventoryAllotmentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "HiredVehicleBills",
                newName: "HiredVehicleBillId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FuelBills",
                newName: "FuelBillId");

            migrationBuilder.RenameColumn(
                name: "FleetStrength",
                table: "FleetStrengths",
                newName: "FleetStrengthValue");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Districts",
                newName: "DistrictId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ContractualBills",
                newName: "ContractualBillId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BillClaims",
                newName: "BillClaimId");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentDeptId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("ALTER TABLE \"Officers\" ALTER COLUMN \"OfficerId\" DROP DEFAULT;");
            migrationBuilder.Sql("ALTER TABLE \"Officers\" ALTER COLUMN \"OfficerId\" TYPE integer USING \"OfficerId\"::integer;");

            migrationBuilder.AlterColumn<int>(
                name: "OfficerId",
                table: "Officers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentDeptId",
                table: "Officers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "OfficerIdString",
                table: "Officers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Officers",
                table: "Officers",
                column: "OfficerId");

            migrationBuilder.CreateTable(
                name: "PetrolPump",
                columns: table => new
                {
                    PetrolPumpId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GSTIN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetrolPump", x => x.PetrolPumpId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offices_DepartmentDeptId",
                table: "Offices",
                column: "DepartmentDeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DepartmentDeptId",
                table: "Officers",
                column: "DepartmentDeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_Departments_DeptId",
                table: "FleetStrengths",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_Districts_DistrictId",
                table: "FleetStrengths",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_VehicleTypes_VehicleTypeId",
                table: "FleetStrengths",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "VehicleTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Departments_DepartmentDeptId",
                table: "Officers",
                column: "DepartmentDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Departments_DepartmentDeptId",
                table: "Offices",
                column: "DepartmentDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "TehsilId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "ManufacturerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "VehicleTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "OfficerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_Departments_DeptId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_Districts_DistrictId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_FleetStrengths_VehicleTypes_VehicleTypeId",
                table: "FleetStrengths");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Departments_DepartmentDeptId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Departments_DepartmentDeptId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "PetrolPump");

            migrationBuilder.DropIndex(
                name: "IX_Offices_DepartmentDeptId",
                table: "Offices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Officers",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DepartmentDeptId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "OfficerIdString",
                table: "Officers");

            migrationBuilder.RenameColumn(
                name: "VehicleTypeId",
                table: "VehicleTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "VehicleTransferId",
                table: "VehicleTransfers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "VehicleModels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "VehicleCondemnationId",
                table: "VehicleCondemnations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TehsilId",
                table: "Tehsils",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "StockTransactionId",
                table: "StockTransactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OfficeId",
                table: "Offices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DepartmentDeptId",
                table: "Officers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ManufacturerId",
                table: "Manufacturers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MaintenanceBillId",
                table: "MaintenanceBills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "InventoryItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InventoryAllotmentId",
                table: "InventoryAllotments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "HiredVehicleBillId",
                table: "HiredVehicleBills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FuelBillId",
                table: "FuelBills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FleetStrengthValue",
                table: "FleetStrengths",
                newName: "FleetStrength");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Districts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ContractualBillId",
                table: "ContractualBills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BillClaimId",
                table: "BillClaims",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "OfficerId",
                table: "Officers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Officers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Officers",
                table: "Officers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_DeptId",
                table: "Offices",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DeptId",
                table: "Officers",
                column: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_Departments_DeptId",
                table: "FleetStrengths",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_Districts_DistrictId",
                table: "FleetStrengths",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FleetStrengths_VehicleTypes_VehicleTypeId",
                table: "FleetStrengths",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Departments_DeptId",
                table: "Offices",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Officers_OfficerId",
                table: "Vehicles",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
