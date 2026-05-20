using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleNOCDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleNOCDetail",
                columns: table => new
                {
                    VehicleNOCDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNOC = table.Column<string>(type: "text", nullable: false),
                    NOC_IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NOC_ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleNOCDetail", x => x.VehicleNOCDetailId);
                    table.ForeignKey(
                        name: "FK_VehicleNOCDetail_Vehicles_VehicleInfoId",
                        column: x => x.VehicleInfoId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RequisitionDeptId",
                table: "Vehicles",
                column: "RequisitionDeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RequisitionOfficeId",
                table: "Vehicles",
                column: "RequisitionOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleNOCDetail_VehicleInfoId",
                table: "VehicleNOCDetail",
                column: "VehicleInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Departments_RequisitionDeptId",
                table: "Vehicles",
                column: "RequisitionDeptId",
                principalTable: "Departments",
                principalColumn: "DeptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Offices_RequisitionOfficeId",
                table: "Vehicles",
                column: "RequisitionOfficeId",
                principalTable: "Offices",
                principalColumn: "OfficeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Departments_RequisitionDeptId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Offices_RequisitionOfficeId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleNOCDetail");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_RequisitionDeptId",
                table: "Vehicles");

        }
    }
}
