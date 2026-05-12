using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Departments",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Departments",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Departments",
                newName: "DepartmentCode");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Departments",
                newName: "DeptId");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Vehicles",
                type: "integer",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Officers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "BillClaims",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCondemnations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    CondemnationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CondemnationOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CondemnationOrderPath = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    AuctionStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuctionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuctionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCondemnations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCondemnations_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    FromOfficeId = table.Column<int>(type: "integer", nullable: false),
                    ToOfficeId = table.Column<int>(type: "integer", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransferOrderNumber = table.Column<string>(type: "text", nullable: false),
                    TransferOrderPath = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTransfers_Offices_FromOfficeId",
                        column: x => x.FromOfficeId,
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTransfers_Offices_ToOfficeId",
                        column: x => x.ToOfficeId,
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTransfers_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAllotments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    AllottedByUserId = table.Column<int>(type: "integer", nullable: false),
                    OdometerReading = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    AllotmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAllotments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAllotments_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAllotments_Users_AllottedByUserId",
                        column: x => x.AllottedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAllotments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    BillNumber = table.Column<string>(type: "text", nullable: true),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PetrolFuelLimit", "PetrolMaintenanceLimit" },
                values: new object[] { 0m, 0m });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ProjectId",
                table: "Vehicles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BillClaims_VehicleId",
                table: "BillClaims",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAllotments_AllottedByUserId",
                table: "InventoryAllotments",
                column: "AllottedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAllotments_InventoryItemId",
                table: "InventoryAllotments",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAllotments_VehicleId",
                table: "InventoryAllotments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_InventoryItemId",
                table: "StockTransactions",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_UserId",
                table: "StockTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCondemnations_VehicleId",
                table: "VehicleCondemnations",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTransfers_FromOfficeId",
                table: "VehicleTransfers",
                column: "FromOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTransfers_ToOfficeId",
                table: "VehicleTransfers",
                column: "ToOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTransfers_VehicleId",
                table: "VehicleTransfers",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillClaims_Vehicles_VehicleId",
                table: "BillClaims",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillClaims_Vehicles_VehicleId",
                table: "BillClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Projects_ProjectId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "InventoryAllotments");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "VehicleCondemnations");

            migrationBuilder.DropTable(
                name: "VehicleTransfers");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ProjectId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_BillClaims_VehicleId",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PetrolFuelLimit",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PetrolMaintenanceLimit",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DieselFuelLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DieselMaintenanceLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "PetrolFuelLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "PetrolMaintenanceLimit",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "BillClaims");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Departments",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "Departments",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DepartmentCode",
                table: "Departments",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                table: "Departments",
                newName: "Id");
        }
    }
}
