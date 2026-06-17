using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FridgeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCatalogSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fridges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fridges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FridgeId = table.Column<int>(type: "int", nullable: false),
                    ProductDefinitionId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    IsOpened = table.Column<bool>(type: "bit", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuickAmounts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsQuickAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProductDefinitions",
                columns: new[] { "Id", "Category", "DefaultUnit", "IsActive", "IsQuickAdd", "Name", "QuickAmounts", "SubCategory" },
                values: new object[,]
                {
                    { 1, "Kahvaltilik", "adet", true, true, "Yumurta", "[6,10,12,15,20,30]", null },
                    { 2, "Sut Urunleri", "litre", true, true, "Sut", "[0.5,1,2]", null },
                    { 3, "Sut Urunleri", "gram", true, true, "Yogurt", "[250,500,1000]", null },
                    { 4, "Kahvaltilik", "gram", true, true, "Peynir", "[100,250,500]", null },
                    { 5, "Kahvaltilik", "gram", true, true, "Tereyagi", "[100,250,500]", null },
                    { 6, "Et ve Tavuk", "gram", true, true, "Tavuk", "[250,500,1000]", null },
                    { 7, "Et ve Tavuk", "gram", true, true, "Kiyma", "[250,500,1000]", null },
                    { 8, "Sebze", "adet", true, true, "Domates", "[2,4,6,8]", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fridges");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ProductDefinitions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
