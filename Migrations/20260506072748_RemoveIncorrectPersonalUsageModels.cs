using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIncorrectPersonalUsageModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalUsageDetails");

            migrationBuilder.DropTable(
                name: "PersonalUsagePlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalUsagePlans",
                columns: table => new
                {
                    PersonalUsagePlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfficerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DetailHead = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinancialYear = table.Column<int>(type: "integer", nullable: false),
                    GRNBillNo = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MajorHead = table.Column<string>(type: "text", nullable: false),
                    MinorHead = table.Column<string>(type: "text", nullable: false),
                    MonthId = table.Column<int>(type: "integer", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PlanAmount = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    ReceiptAmount = table.Column<int>(type: "integer", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SelectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubHead = table.Column<string>(type: "text", nullable: false),
                    SubMajorHead = table.Column<string>(type: "text", nullable: false),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalUsagePlans", x => x.PersonalUsagePlanId);
                    table.ForeignKey(
                        name: "FK_PersonalUsagePlans_Officers_OfficerId",
                        column: x => x.OfficerId,
                        principalTable: "Officers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalUsageDetails",
                columns: table => new
                {
                    PersonalUsageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonalUsagePlanId = table.Column<int>(type: "integer", nullable: false),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinancialYear = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsTemp = table.Column<bool>(type: "boolean", nullable: true),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    KmUsed = table.Column<int>(type: "integer", nullable: false),
                    MonthId = table.Column<int>(type: "integer", nullable: false),
                    OdometerFrom = table.Column<long>(type: "bigint", nullable: false),
                    OdometerTo = table.Column<long>(type: "bigint", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PersonalUseDetails = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<string>(type: "text", nullable: false),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalUsageDetails", x => x.PersonalUsageId);
                    table.ForeignKey(
                        name: "FK_PersonalUsageDetails_PersonalUsagePlans_PersonalUsagePlanId",
                        column: x => x.PersonalUsagePlanId,
                        principalTable: "PersonalUsagePlans",
                        principalColumn: "PersonalUsagePlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonalUsageDetails_Vehicles_VehicleInfoId",
                        column: x => x.VehicleInfoId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalUsageDetails_PersonalUsagePlanId",
                table: "PersonalUsageDetails",
                column: "PersonalUsagePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalUsageDetails_VehicleInfoId",
                table: "PersonalUsageDetails",
                column: "VehicleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalUsagePlans_OfficerId",
                table: "PersonalUsagePlans",
                column: "OfficerId");
        }
    }
}
