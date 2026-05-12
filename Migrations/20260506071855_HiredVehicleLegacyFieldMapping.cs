using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class HiredVehicleLegacyFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HiredVehicles",
                columns: table => new
                {
                    HiredVehicleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "text", nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    OfficeId = table.Column<int>(type: "integer", nullable: false),
                    VehicleTypeId = table.Column<int>(type: "integer", nullable: false),
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    ModelId = table.Column<int>(type: "integer", nullable: false),
                    SeatingCapacity = table.Column<int>(type: "integer", nullable: false),
                    FuelUsed = table.Column<string>(type: "text", nullable: false),
                    ContractorName = table.Column<string>(type: "text", nullable: false),
                    ContractorPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    BillDateFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillDateTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KMCovered = table.Column<string>(type: "text", nullable: false),
                    BillAmount = table.Column<int>(type: "integer", nullable: false),
                    Noofvehicles = table.Column<int>(type: "integer", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiredVehicles", x => x.HiredVehicleId);
                    table.ForeignKey(
                        name: "FK_HiredVehicles_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiredVehicles_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiredVehicles_VehicleModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "VehicleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiredVehicles_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicles_ManufacturerId",
                table: "HiredVehicles",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicles_ModelId",
                table: "HiredVehicles",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicles_OfficeId",
                table: "HiredVehicles",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_HiredVehicles_VehicleTypeId",
                table: "HiredVehicles",
                column: "VehicleTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HiredVehicles");
        }
    }
}
