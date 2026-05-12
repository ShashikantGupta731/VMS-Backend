using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class TehsilFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tehsils_Districts_DistrictId",
                table: "Tehsils");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tehsils",
                newName: "TehsilName");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Tehsils",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Tehsils",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tehsils_Districts_DistrictId",
                table: "Tehsils",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tehsils_Districts_DistrictId",
                table: "Tehsils");

            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Tehsils");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Tehsils");

            migrationBuilder.RenameColumn(
                name: "TehsilName",
                table: "Tehsils",
                newName: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Tehsils_Districts_DistrictId",
                table: "Tehsils",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
