using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfficeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DDOCode",
                table: "Offices");

            migrationBuilder.RenameColumn(
                name: "OfficeNameWithDdo",
                table: "Offices",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Offices",
                newName: "Enabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Offices",
                newName: "OfficeNameWithDdo");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Offices",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "DDOCode",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
