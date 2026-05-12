using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SanctionPermissionFile",
                table: "MaintenanceBills",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SanctionPermissionFile",
                table: "FuelBills",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "BillClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ForwardedToTreasury",
                table: "BillClaims",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SanctionAuthority",
                table: "BillClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SanctionOrderDate",
                table: "BillClaims",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SanctionOrderNo",
                table: "BillClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubVoucherDescription",
                table: "BillClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubVoucherNo",
                table: "BillClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "BillClaims",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ContractualBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillPeriodFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillPeriodTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DdoCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VehicleNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractualBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractualBills_BillClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "BillClaims",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractualBills_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HiredVehicleBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OfficeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContractorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContractorPhone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    VehicleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NoOfVehicles = table.Column<int>(type: "integer", nullable: false),
                    HiredFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HiredTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KmCovered = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiredVehicleBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiredVehicleBills_BillClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "BillClaims",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HiredVehicleBills_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedDate", "DepartmentId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 40, true, "State Road Maintenance" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 40, true, "NHAI Punjab Segment" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, true, "Smart City Ludhiana" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, true, "Border Area Development" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractualBills_ClaimId",
                table: "ContractualBills",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractualBills_CreatedById",
                table: "ContractualBills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicleBills_ClaimId",
                table: "HiredVehicleBills",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicleBills_CreatedById",
                table: "HiredVehicleBills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DepartmentId",
                table: "Projects",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractualBills");

            migrationBuilder.DropTable(
                name: "HiredVehicleBills");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropColumn(
                name: "SanctionPermissionFile",
                table: "MaintenanceBills");

            migrationBuilder.DropColumn(
                name: "SanctionPermissionFile",
                table: "FuelBills");

            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "ForwardedToTreasury",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "SanctionAuthority",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "SanctionOrderDate",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "SanctionOrderNo",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "SubVoucherDescription",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "SubVoucherNo",
                table: "BillClaims");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "BillClaims");
        }
    }
}
