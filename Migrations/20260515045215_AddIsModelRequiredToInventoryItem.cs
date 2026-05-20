using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsModelRequiredToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsModelRequired",
                table: "InventoryItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MiscellaneousBills",
                columns: table => new
                {
                    MiscellaneousBillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BillNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false),
                    ModelNumber = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiscellaneousBills", x => x.MiscellaneousBillId);
                    table.ForeignKey(
                        name: "FK_MiscellaneousBills_BillClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "BillClaims",
                        principalColumn: "BillClaimId");
                    table.ForeignKey(
                        name: "FK_MiscellaneousBills_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiscellaneousBills_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousBills_ClaimId",
                table: "MiscellaneousBills",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousBills_CreatedById",
                table: "MiscellaneousBills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MiscellaneousBills_InventoryItemId",
                table: "MiscellaneousBills",
                column: "InventoryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiscellaneousBills");

            migrationBuilder.DropColumn(
                name: "IsModelRequired",
                table: "InventoryItems");
        }
    }
}
