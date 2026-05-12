using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DistrictFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Districts",
                newName: "DistrictName");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Districts",
                newName: "DistAbbre");

            migrationBuilder.AddColumn<DateTime>(
                name: "PDate",
                table: "Districts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TDate",
                table: "Districts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DistAbbre", "PDate", "TDate" },
                values: new object[] { "AMR", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DistAbbre", "PDate", "TDate" },
                values: new object[] { "BNA", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PDate", "TDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "FDK", "Faridkot", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "FGS", "Fatehgarh Sahib", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "FZK", "Fazilka", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "FZR", "Ferozepur", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "GSP", "Gurdaspur", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "HSP", "Hoshiarpur", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "JAL", "Jalandhar", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "KPT", "Kapurthala", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "LDH", "Ludhiana", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "MKT", "Malerkotla", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "MS", "Mansa", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "MG", "Moga", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "MKS", "Muktsar Sahib", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "ND", "New Delhi", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "PTK", "Pathankot", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "PTL", "Patiala", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "RUP", "Rupnagar", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "SGR", "Sangrur", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "SAS", "SAS Nagar (Mohali)", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "SBS", "Shaheed Bhagat Singh Nagar", null, null });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "DistAbbre", "DistrictName", "PDate", "TDate" },
                values: new object[] { "TT", "Tarn Taran", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDate",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "TDate",
                table: "Districts");

            migrationBuilder.RenameColumn(
                name: "DistrictName",
                table: "Districts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DistAbbre",
                table: "Districts",
                newName: "Code");

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "ASR");

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Code",
                value: "BNL");

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "CH", "Chandigarh" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Name" },
                values: new object[] { "FDK", "Faridkot" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "Name" },
                values: new object[] { "FGS", "Fatehgarh Sahib" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "Name" },
                values: new object[] { "FZK", "Fazilka" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "Name" },
                values: new object[] { "FZR", "Ferozepur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "Name" },
                values: new object[] { "GSP", "Gurdaspur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "Name" },
                values: new object[] { "HSP", "Hoshiarpur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "Name" },
                values: new object[] { "JAL", "Jalandhar" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "Name" },
                values: new object[] { "KPT", "Kapurthala" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Code", "Name" },
                values: new object[] { "LDH", "Ludhiana" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Code", "Name" },
                values: new object[] { "MKT", "Malerkotla" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Code", "Name" },
                values: new object[] { "MS", "Mansa" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Code", "Name" },
                values: new object[] { "MG", "Moga" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Code", "Name" },
                values: new object[] { "MKS", "Muktsar Sahib" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ND", "New Delhi" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Code", "Name" },
                values: new object[] { "PTK", "Pathankot" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Code", "Name" },
                values: new object[] { "PTL", "Patiala" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Code", "Name" },
                values: new object[] { "RUP", "Rupnagar" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Code", "Name" },
                values: new object[] { "SGR", "Sangrur" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Code", "Name" },
                values: new object[] { "SAS", "SAS Nagar (Mohali)" });

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Code", "Name" },
                values: new object[] { "SBS", "Shaheed Bhagat Singh Nagar" });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[] { 25, "TT", true, "Tarn Taran" });
        }
    }
}
