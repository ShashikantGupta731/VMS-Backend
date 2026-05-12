using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ContractualRequisiteLegacyFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractualRequisites",
                columns: table => new
                {
                    ContractualRequisiteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "text", nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillDateFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillDateTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DDOCode = table.Column<string>(type: "text", nullable: false),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    BillAmount = table.Column<int>(type: "integer", nullable: false),
                    IsContractual = table.Column<bool>(type: "boolean", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractualRequisites", x => x.ContractualRequisiteId);
                    table.ForeignKey(
                        name: "FK_ContractualRequisites_Vehicles_VehicleInfoId",
                        column: x => x.VehicleInfoId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractualRequisites_VehicleInfoId",
                table: "ContractualRequisites",
                column: "VehicleInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractualRequisites");
        }
    }
}
