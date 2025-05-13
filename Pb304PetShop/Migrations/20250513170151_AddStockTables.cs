using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pb304PetShop.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockByColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockByColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockByColors_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockByColors_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockBySizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockByColorId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBySizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBySizes_StockByColors_StockByColorId",
                        column: x => x.StockByColorId,
                        principalTable: "StockByColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockByColors_ColorId",
                table: "StockByColors",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_StockByColors_ProductId",
                table: "StockByColors",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBySizes_StockByColorId",
                table: "StockBySizes",
                column: "StockByColorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockBySizes");

            migrationBuilder.DropTable(
                name: "StockByColors");
        }
    }
}
