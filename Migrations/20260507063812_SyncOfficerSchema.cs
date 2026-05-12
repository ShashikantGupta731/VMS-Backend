using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SyncOfficerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Departments_DepartmentDeptId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DepartmentDeptId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptId",
                table: "Officers");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DeptId",
                table: "Officers",
                column: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Departments_DeptId",
                table: "Officers",
                column: "DeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Departments_DeptId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DeptId",
                table: "Officers");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentDeptId",
                table: "Officers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DepartmentDeptId",
                table: "Officers",
                column: "DepartmentDeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Departments_DepartmentDeptId",
                table: "Officers",
                column: "DepartmentDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Designations_DesignationId",
                table: "Officers",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "DesignationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
