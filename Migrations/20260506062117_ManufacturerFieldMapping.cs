using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ManufacturerFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Manufacturers",
                newName: "ManufacturerName");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Manufacturers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Manufacturers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Manufacturers");

            migrationBuilder.RenameColumn(
                name: "ManufacturerName",
                table: "Manufacturers",
                newName: "Name");
        }
    }
}
