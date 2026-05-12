using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UserFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "PhoneNo");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Users",
                newName: "DeptId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentDeptId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentDeptId",
                table: "Users",
                column: "DepartmentDeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentDeptId",
                table: "Users",
                column: "DepartmentDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentDeptId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentDeptId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                table: "Users",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DeptId");
        }
    }
}
