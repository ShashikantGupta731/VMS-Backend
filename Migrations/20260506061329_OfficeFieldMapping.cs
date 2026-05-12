using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class OfficeFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Departments_DepartmentId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Offices_DepartmentId",
                table: "Offices");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Offices",
                newName: "OfficeTypeOther");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Offices",
                newName: "OfficeTypeId");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Offices",
                newName: "OfficeNameWithDdo");

            migrationBuilder.AlterColumn<int>(
                name: "TehsilId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeptId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OfficeAbbreviation",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeAddress",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeName",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Offices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Offices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offices_DeptId",
                table: "Offices",
                column: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Departments_DeptId",
                table: "Offices",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Departments_DeptId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Offices_DeptId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "OfficeAbbreviation",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "OfficeAddress",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "OfficeName",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Offices");

            migrationBuilder.RenameColumn(
                name: "OfficeTypeOther",
                table: "Offices",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OfficeTypeId",
                table: "Offices",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "OfficeNameWithDdo",
                table: "Offices",
                newName: "Address");

            migrationBuilder.AlterColumn<int>(
                name: "TehsilId",
                table: "Offices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_DepartmentId",
                table: "Offices",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Departments_DepartmentId",
                table: "Offices",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "Id");
        }
    }
}
