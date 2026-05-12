using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FuelRelatedModelsLegacyFieldMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuelEntries",
                columns: table => new
                {
                    FuelEntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "text", nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    OdometerReading = table.Column<int>(type: "integer", nullable: false),
                    Permission = table.Column<string>(type: "text", nullable: false),
                    Nocfile = table.Column<string>(type: "text", nullable: false),
                    NocIssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NocExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SanctionAuthorityMobileNo = table.Column<string>(type: "text", nullable: false),
                    IsPersonalUsed = table.Column<bool>(type: "boolean", nullable: true),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelEntries", x => x.FuelEntryId);
                    table.ForeignKey(
                        name: "FK_FuelEntries_Vehicles_VehicleInfoId",
                        column: x => x.VehicleInfoId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelMaintenances",
                columns: table => new
                {
                    FuelMaintenanceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "text", nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    OdometerReading = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    MaintenanceType = table.Column<string>(type: "text", nullable: false),
                    SubVoucherNo = table.Column<string>(type: "text", nullable: false),
                    SubVoucherDesc = table.Column<string>(type: "text", nullable: false),
                    ExpenditureDetails = table.Column<string>(type: "text", nullable: false),
                    SanctionOrderNo = table.Column<string>(type: "text", nullable: false),
                    SanctionOrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SanctionAuthority = table.Column<string>(type: "text", nullable: false),
                    FirmName = table.Column<string>(type: "text", nullable: false),
                    FwdToTreasury = table.Column<bool>(type: "boolean", nullable: true),
                    ClaimInfo = table.Column<string>(type: "text", nullable: false),
                    IsProduction = table.Column<bool>(type: "boolean", nullable: true),
                    IsGrantInAidBill = table.Column<bool>(type: "boolean", nullable: true),
                    GrantInAidPeriod = table.Column<string>(type: "text", nullable: false),
                    SanctionedBy = table.Column<string>(type: "text", nullable: false),
                    FDSanctionLetterNo = table.Column<string>(type: "text", nullable: false),
                    DateOfIssue = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    IsBulkBill = table.Column<bool>(type: "boolean", nullable: true),
                    IsSupplementaryBill = table.Column<bool>(type: "boolean", nullable: true),
                    ParentClaimId = table.Column<string>(type: "text", nullable: false),
                    SupplementaryAllotmentDone = table.Column<bool>(type: "boolean", nullable: true),
                    IFMSStatus = table.Column<int>(type: "integer", nullable: false),
                    IFMSBillNo = table.Column<long>(type: "bigint", nullable: true),
                    IsNewIFMS = table.Column<bool>(type: "boolean", nullable: true),
                    BillInfoDetail = table.Column<string>(type: "text", nullable: false),
                    VMSRefNo = table.Column<long>(type: "bigint", nullable: true),
                    BillSubmittedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IncomeTaxAmount = table.Column<int>(type: "integer", nullable: false),
                    ClaimNo = table.Column<string>(type: "text", nullable: false),
                    ClaimResponseId = table.Column<string>(type: "text", nullable: false),
                    ClaimVerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelMaintenances", x => x.FuelMaintenanceId);
                    table.ForeignKey(
                        name: "FK_FuelMaintenances_Vehicles_VehicleInfoId",
                        column: x => x.VehicleInfoId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalUsagePlans",
                columns: table => new
                {
                    PersonalUsagePlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfficerId = table.Column<int>(type: "integer", nullable: false),
                    PlanAmount = table.Column<int>(type: "integer", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    SelectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinancialYear = table.Column<int>(type: "integer", nullable: false),
                    MonthId = table.Column<int>(type: "integer", nullable: false),
                    GRNBillNo = table.Column<string>(type: "text", nullable: false),
                    MajorHead = table.Column<string>(type: "text", nullable: false),
                    SubMajorHead = table.Column<string>(type: "text", nullable: false),
                    MinorHead = table.Column<string>(type: "text", nullable: false),
                    SubHead = table.Column<string>(type: "text", nullable: false),
                    DetailHead = table.Column<string>(type: "text", nullable: false),
                    ReceiptAmount = table.Column<int>(type: "integer", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                    RecordId = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    PersonalUsagePlanId = table.Column<int>(type: "integer", nullable: false),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    OdometerFrom = table.Column<long>(type: "bigint", nullable: false),
                    OdometerTo = table.Column<long>(type: "bigint", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KmUsed = table.Column<int>(type: "integer", nullable: false),
                    PersonalUseDetails = table.Column<string>(type: "text", nullable: false),
                    FinancialYear = table.Column<int>(type: "integer", nullable: false),
                    MonthId = table.Column<int>(type: "integer", nullable: false),
                    IsTemp = table.Column<bool>(type: "boolean", nullable: true),
                    PDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "IX_FuelEntries_VehicleInfoId",
                table: "FuelEntries",
                column: "VehicleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelMaintenances_VehicleInfoId",
                table: "FuelMaintenances",
                column: "VehicleInfoId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelEntries");

            migrationBuilder.DropTable(
                name: "FuelMaintenances");

            migrationBuilder.DropTable(
                name: "PersonalUsageDetails");

            migrationBuilder.DropTable(
                name: "PersonalUsagePlans");
        }
    }
}
