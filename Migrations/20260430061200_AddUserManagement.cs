using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FdApprovalPath",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FleetStrengthLetterPath",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HRMSCode",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficerName",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationCertificatePath",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehiclePhotoPath",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsNonTreasuryDDO",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManagedDdos",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FdApprovalPath",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FleetStrengthLetterPath",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "HRMSCode",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "OfficerName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RegistrationCertificatePath",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehiclePhotoPath",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsNonTreasuryDDO",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagedDdos",
                table: "Users");
        }
    }
}
