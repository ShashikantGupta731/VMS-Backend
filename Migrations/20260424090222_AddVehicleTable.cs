using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PurchasedNewVehicle = table.Column<string>(type: "text", nullable: false),
                    OfficeName = table.Column<string>(type: "text", nullable: false),
                    CurrentStatus = table.Column<string>(type: "text", nullable: false),
                    VehicleAllocationType = table.Column<string>(type: "text", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    OfficerName = table.Column<string>(type: "text", nullable: false),
                    HrmsCode = table.Column<string>(type: "text", nullable: false),
                    DriverType = table.Column<string>(type: "text", nullable: false),
                    DriverName = table.Column<string>(type: "text", nullable: false),
                    DriverContactNumber = table.Column<string>(type: "text", nullable: false),
                    ContractorName = table.Column<string>(type: "text", nullable: false),
                    ContractorContactNumber = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    VehicleOwnerOffice = table.Column<string>(type: "text", nullable: false),
                    RegistrationType = table.Column<string>(type: "text", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: false),
                    ManufactureYear = table.Column<string>(type: "text", nullable: false),
                    SeatingCapacity = table.Column<int>(type: "integer", nullable: true),
                    VehicleType = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    VehiclePhoto = table.Column<string>(type: "text", nullable: false),
                    RegistrationCertificate = table.Column<string>(type: "text", nullable: false),
                    ChassisNumber = table.Column<string>(type: "text", nullable: false),
                    VehicleCost = table.Column<decimal>(type: "numeric", nullable: true),
                    FuelUsed = table.Column<string>(type: "text", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FitnessUpto = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KmsCovered = table.Column<int>(type: "integer", nullable: true),
                    FuelCostLast3Months = table.Column<decimal>(type: "numeric", nullable: true),
                    FuelLitresLast3Months = table.Column<decimal>(type: "numeric", nullable: true),
                    MaintenanceCostLast3Months = table.Column<decimal>(type: "numeric", nullable: true),
                    IsTyreOriginal = table.Column<string>(type: "text", nullable: false),
                    TyreChangedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TyreChangedMeterReading = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System administrator with full access", "Admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Regular user with standard access", "User" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vehicle agent with limited access", "Agent" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CreatedByUserId",
                table: "Vehicles",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
