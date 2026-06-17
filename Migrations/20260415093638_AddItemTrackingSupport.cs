using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddItemTrackingSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrackingType",
                table: "ProductDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ApproximateStatus",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackingType",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 1,
                column: "TrackingType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 2,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 3,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 4,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 5,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 6,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 7,
                column: "TrackingType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProductDefinitions",
                keyColumn: "Id",
                keyValue: 8,
                column: "TrackingType",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingType",
                table: "ProductDefinitions");

            migrationBuilder.DropColumn(
                name: "ApproximateStatus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TrackingType",
                table: "Items");
        }
    }
}
