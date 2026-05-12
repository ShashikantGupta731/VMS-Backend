using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DDORegistrationNo",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "CreatedAt", "Description", "Name" },
                values: new object[] { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Revenue Officer Level", "ROFC" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "DDOCode", "DDORegistrationNo", "DepartmentDeptId", "DeptId", "DistrictId", "EmailId", "Enabled", "FailedAttempts", "FirstName", "IsGuest", "IsNonTreasuryDDO", "LastName", "LockUntil", "ManagedDdos", "MiddleName", "Name", "PasswordHash", "PhoneNo", "Username" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ADMIN", null, null, null, null, null, true, 0, null, false, false, null, null, null, null, "System Administrator", "$2a$11$v1eUXEH5k565XSNl.0exsOTBEfRQqB8Zzj/6WYFFWNaRLWkBog5PG", null, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 2, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "DDORegistrationNo",
                table: "Users");
        }
    }
}
