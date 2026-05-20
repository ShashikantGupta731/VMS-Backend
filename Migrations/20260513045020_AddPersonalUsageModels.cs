using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalUsageModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalUsagePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleInfoId = table.Column<int>(type: "integer", nullable: false),
                    VehicleNo = table.Column<string>(type: "text", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    RecordId = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalUsagePlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonalUsagePlanId = table.Column<int>(type: "integer", nullable: false),
                    OfficerId = table.Column<string>(type: "text", nullable: false),
                    DateOfUse = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OdometerFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    OdometerTo = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalUsageLogs_PersonalUsagePlans_PersonalUsagePlanId",
                        column: x => x.PersonalUsagePlanId,
                        principalTable: "PersonalUsagePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalUsageLogs_PersonalUsagePlanId",
                table: "PersonalUsageLogs",
                column: "PersonalUsagePlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalUsageLogs");

            migrationBuilder.DropTable(
                name: "PersonalUsagePlans");
        }
    }
}
