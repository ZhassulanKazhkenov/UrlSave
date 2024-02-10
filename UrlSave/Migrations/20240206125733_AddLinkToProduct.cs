using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSave.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URL",
                table: "Products");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
