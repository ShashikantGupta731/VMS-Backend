using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedActualData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "AGR", "Agriculture" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "AHD", "Animal Husbandry Dairy Development and Fisheries" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "COP", "Cooperation" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "DSW", "Defence Services Welfare" });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedDate", "IsActive", "Name" },
                values: new object[,]
                {
                    { 5, "ELE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Elections" },
                    { 6, "EGT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Employment Generation and Training" },
                    { 7, "ET", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Excise and Taxation" },
                    { 8, "FIN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Finance" },
                    { 9, "FCS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Food Civil Supplies and Consumer Affairs" },
                    { 10, "FWL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Forest and Wild Life Preservation" },
                    { 11, "GA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "General Administration" },
                    { 12, "GR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Governance Reforms" },
                    { 13, "HFW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Health & Family Welfare" },
                    { 14, "HOME", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Home Affairs" },
                    { 15, "HORT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Horticulture" },
                    { 16, "HUD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Housing and Urban Development" },
                    { 17, "IC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Industries and Commerce" },
                    { 18, "IPR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Information and Public Relation" },
                    { 19, "IRR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Irrigation" },
                    { 20, "JAIL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Jails" },
                    { 21, "LAB", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Labour" },
                    { 22, "LLA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Legal and Legislative Affairs" },
                    { 23, "LG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Local Government" },
                    { 24, "LBP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Lok Bhavan Punjab" },
                    { 25, "MER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Medical Education and Research" },
                    { 26, "PA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Parliamentary Affairs" },
                    { 27, "PER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Personnel" },
                    { 28, "PLAN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Planning" },
                    { 29, "POW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Power" },
                    { 30, "PS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Printing and Stationery" },
                    { 31, "PW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Public Works" },
                    { 32, "RRD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Revenue Rehabilitation & Disaster Management" },
                    { 33, "RDP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Rural Development and Panchayat" },
                    { 34, "SE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "School Education" },
                    { 35, "STE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Science, Technology & Environment" },
                    { 36, "SSW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Social Security and Development of Women and Children" },
                    { 37, "SYS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Sports and Youth Services" },
                    { 38, "TEI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Technical Education and Industrial Training" },
                    { 39, "TCA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Tourism and Cultural Affairs" },
                    { 40, "TRA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Transport" },
                    { 41, "VIG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Vigilance" },
                    { 42, "WSS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Water Supply and Sanitation" },
                    { 43, "WBC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Welfare of SCs & BCs" }
                });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ASR", "Amritsar" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "BNL", "Barnala" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "BTI", "Bathinda" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "CH", "Chandigarh" });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 5, "FDK", true, "Faridkot" },
                    { 6, "FGS", true, "Fatehgarh Sahib" },
                    { 7, "FZK", true, "Fazilka" },
                    { 8, "FZR", true, "Ferozepur" },
                    { 9, "GSP", true, "Gurdaspur" },
                    { 10, "HSP", true, "Hoshiarpur" },
                    { 11, "JAL", true, "Jalandhar" },
                    { 12, "KPT", true, "Kapurthala" },
                    { 13, "LDH", true, "Ludhiana" },
                    { 14, "MKT", true, "Malerkotla" },
                    { 15, "MS", true, "Mansa" },
                    { 16, "MG", true, "Moga" },
                    { 17, "MKS", true, "Muktsar Sahib" },
                    { 18, "ND", true, "New Delhi" },
                    { 19, "PTK", true, "Pathankot" },
                    { 20, "PTL", true, "Patiala" },
                    { 21, "RUP", true, "Rupnagar" },
                    { 22, "SGR", true, "Sangrur" },
                    { 23, "SAS", true, "SAS Nagar (Mohali)" },
                    { 24, "SBS", true, "Shaheed Bhagat Singh Nagar" },
                    { 25, "TT", true, "Tarn Taran" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "POL", "Police Department" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "HLT", "Health Department" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "EDU", "Education Department" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "FIN", "Finance Department" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "JP", "Jaipur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "JD", "Jodhpur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "UD", "Udaipur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "AJ", "Ajmer" });
        }
    }
}
