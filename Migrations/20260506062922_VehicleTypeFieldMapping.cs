using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class VehicleTypeFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "VehicleTypes",
                newName: "VehicleTypeName");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "VehicleTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "VehicleTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleLifeKM",
                table: "VehicleTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleLifeYears",
                table: "VehicleTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VehicleTypeExample",
                table: "VehicleTypes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDate",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "VehicleLifeKM",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "VehicleLifeYears",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "VehicleTypeExample",
                table: "VehicleTypes");

            migrationBuilder.RenameColumn(
                name: "VehicleTypeName",
                table: "VehicleTypes",
                newName: "Name");
        }
    }
}
