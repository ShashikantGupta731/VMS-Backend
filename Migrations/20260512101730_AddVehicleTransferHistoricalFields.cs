using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTransferHistoricalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromAllocationType",
                table: "VehicleTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromDdoCode",
                table: "VehicleTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromDesignationName",
                table: "VehicleTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromOfficerName",
                table: "VehicleTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToDdoCode",
                table: "VehicleTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "VehicleTransfers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromAllocationType",
                table: "VehicleTransfers");

            migrationBuilder.DropColumn(
                name: "FromDdoCode",
                table: "VehicleTransfers");

            migrationBuilder.DropColumn(
                name: "FromDesignationName",
                table: "VehicleTransfers");

            migrationBuilder.DropColumn(
                name: "FromOfficerName",
                table: "VehicleTransfers");

            migrationBuilder.DropColumn(
                name: "ToDdoCode",
                table: "VehicleTransfers");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "VehicleTransfers");
        }
    }
}
