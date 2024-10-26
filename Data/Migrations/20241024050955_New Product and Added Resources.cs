using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingApp.Data.Migrations
{
    public partial class NewProductandAddedResources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Product",
                newName: "ResourceNames");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Product",
                newName: "Time_Cost");

            migrationBuilder.AddColumn<float>(
                name: "Profit",
                table: "Product",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ResourceCounts",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ProductId",
                table: "Resource",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ResourceCounts",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Time_Cost",
                table: "Product",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "ResourceNames",
                table: "Product",
                newName: "Description");
        }
    }
}
