using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class CleanOfficeMappingAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Departments_DepartmentDeptId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Offices_DepartmentDeptId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptId",
                table: "Offices");

            migrationBuilder.AlterColumn<int>(
                name: "TehsilId",
                table: "Offices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DistrictId",
                table: "Offices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DeptId",
                table: "Offices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "TehsilId",
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

            migrationBuilder.AlterColumn<int>(
                name: "TehsilId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DistrictId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeptId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentDeptId",
                table: "Offices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DeptId", "DeptAbbre", "DeptName", "Enabled", "PDate", "TDate" },
                values: new object[,]
                {
                    { 1, "AGR", "Agriculture", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "AHD", "Animal Husbandry Dairy Development and Fisheries", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "COP", "Cooperation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "DSW", "Defence Services Welfare", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "ELE", "Elections", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "EGT", "Employment Generation and Training", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, "ET", "Excise and Taxation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, "FIN", "Finance", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, "FCS", "Food Civil Supplies and Consumer Affairs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, "FWL", "Forest and Wild Life Preservation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 11, "GA", "General Administration", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, "GR", "Governance Reforms", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 13, "HFW", "Health & Family Welfare", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 14, "HOME", "Home Affairs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 15, "HORT", "Horticulture", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 16, "HUD", "Housing and Urban Development", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 17, "IC", "Industries and Commerce", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 18, "IPR", "Information and Public Relation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 19, "IRR", "Irrigation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 20, "JAIL", "Jails", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 21, "LAB", "Labour", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 22, "LLA", "Legal and Legislative Affairs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 23, "LG", "Local Government", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 24, "LBP", "Lok Bhavan Punjab", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 25, "MER", "Medical Education and Research", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 26, "PA", "Parliamentary Affairs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 27, "PER", "Personnel", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 28, "PLAN", "Planning", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 29, "POW", "Power", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 30, "PS", "Printing and Stationery", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 31, "PW", "Public Works", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 32, "RRD", "Revenue Rehabilitation & Disaster Management", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 33, "RDP", "Rural Development and Panchayat", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 34, "SE", "School Education", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 35, "STE", "Science, Technology & Environment", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 36, "SSW", "Social Security and Development of Women and Children", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 37, "SYS", "Sports and Youth Services", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 38, "TEI", "Technical Education and Industrial Training", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 39, "TCA", "Tourism and Cultural Affairs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 40, "TRA", "Transport", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 41, "VIG", "Vigilance", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 42, "WSS", "Water Supply and Sanitation", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 43, "WBC", "Welfare of SCs & BCs", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "DistrictId", "DistAbbre", "DistrictName", "IsActive", "PDate", "TDate" },
                values: new object[,]
                {
                    { 1, "AMR", "Amritsar", true, null, null },
                    { 2, "BNA", "Barnala", true, null, null },
                    { 3, "BTI", "Bathinda", true, null, null },
                    { 4, "FDK", "Faridkot", true, null, null },
                    { 5, "FGS", "Fatehgarh Sahib", true, null, null },
                    { 6, "FZK", "Fazilka", true, null, null },
                    { 7, "FZR", "Ferozepur", true, null, null },
                    { 8, "GSP", "Gurdaspur", true, null, null },
                    { 9, "HSP", "Hoshiarpur", true, null, null },
                    { 10, "JAL", "Jalandhar", true, null, null },
                    { 11, "KPT", "Kapurthala", true, null, null },
                    { 12, "LDH", "Ludhiana", true, null, null },
                    { 13, "MKT", "Malerkotla", true, null, null },
                    { 14, "MS", "Mansa", true, null, null },
                    { 15, "MG", "Moga", true, null, null },
                    { 16, "MKS", "Muktsar Sahib", true, null, null },
                    { 17, "ND", "New Delhi", true, null, null },
                    { 18, "PTK", "Pathankot", true, null, null },
                    { 19, "PTL", "Patiala", true, null, null },
                    { 20, "RUP", "Rupnagar", true, null, null },
                    { 21, "SGR", "Sangrur", true, null, null },
                    { 22, "SAS", "SAS Nagar (Mohali)", true, null, null },
                    { 23, "SBS", "Shaheed Bhagat Singh Nagar", true, null, null },
                    { 24, "TT", "Tarn Taran", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "ProjectId", "DeptId", "FuelLitresPerMonth", "MaintenanceAmtPerAnnum", "MaintenanceAmtPerMonth", "PDate", "ProjectName", "TDate" },
                values: new object[,]
                {
                    { 1, 40, 1000.0, 600000.0, 50000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State Road Maintenance", null },
                    { 2, 40, 1500.0, 900000.0, 75000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NHAI Punjab Segment", null },
                    { 3, 23, 800.0, 480000.0, 40000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smart City Ludhiana", null },
                    { 4, 11, 600.0, 360000.0, 30000.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Border Area Development", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offices_DepartmentDeptId",
                table: "Offices",
                column: "DepartmentDeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Departments_DepartmentDeptId",
                table: "Offices",
                column: "DepartmentDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Districts_DistrictId",
                table: "Offices",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Tehsils_TehsilId",
                table: "Offices",
                column: "TehsilId",
                principalTable: "Tehsils",
                principalColumn: "TehsilId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
