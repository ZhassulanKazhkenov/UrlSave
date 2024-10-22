using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSave.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedInProductPropertyLinkAndAddToLinkPropertyProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Links_LinkId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LinkId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LinkId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Links",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Links_ProductId",
                table: "Links",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Products_ProductId",
                table: "Links",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Products_ProductId",
                table: "Links");

            migrationBuilder.DropIndex(
                name: "IX_Links_ProductId",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Links");

            migrationBuilder.AddColumn<int>(
                name: "LinkId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_LinkId",
                table: "Products",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Links_LinkId",
                table: "Products",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
