using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleModelDetailed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeatingCapacity",
                table: "VehicleModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "VehicleModels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TehsilId",
                table: "Offices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offices_TehsilId",
                table: "Offices",
                column: "TehsilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Offices_TehsilId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "SeatingCapacity",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "TehsilId",
                table: "Offices");
        }
    }
}
