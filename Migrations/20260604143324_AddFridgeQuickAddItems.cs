using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFridgeQuickAddItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FridgeQuickAddItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FridgeId = table.Column<int>(type: "int", nullable: false),
                    ProductDefinitionId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingType = table.Column<int>(type: "int", nullable: false),
                    QuickAmounts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgeQuickAddItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FridgeQuickAddItems_Fridges_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FridgeQuickAddItems_ProductDefinitions_ProductDefinitionId",
                        column: x => x.ProductDefinitionId,
                        principalTable: "ProductDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FridgeQuickAddItems_FridgeId",
                table: "FridgeQuickAddItems",
                column: "FridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeQuickAddItems_ProductDefinitionId",
                table: "FridgeQuickAddItems",
                column: "ProductDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FridgeQuickAddItems");
        }
    }
}
