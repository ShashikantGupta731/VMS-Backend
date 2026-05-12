using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class VehicleModelFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "VehicleModels");

            migrationBuilder.RenameColumn(
                name: "VehicleType",
                table: "VehicleModels",
                newName: "ModelName");

            migrationBuilder.AddColumn<int>(
                name: "VehicleTypeId",
                table: "VehicleModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropIndex(
                name: "IX_VehicleModels_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "VehicleModels",
                newName: "VehicleType");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "VehicleModels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "VehicleModels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_Manufacturers_ManufacturerId",
                table: "VehicleModels",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
