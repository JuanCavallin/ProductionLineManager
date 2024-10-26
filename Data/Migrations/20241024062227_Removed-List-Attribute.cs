using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingApp.Data.Migrations
{
    public partial class RemovedListAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Product_ProductId",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_ProductId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Resource");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Resource",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ProductId",
                table: "Resource",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Product_ProductId",
                table: "Resource",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
