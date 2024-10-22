using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlSave.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceIdToNotificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PriceId",
                table: "Notifications",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Prices_PriceId",
                table: "Notifications",
                column: "PriceId",
                principalTable: "Prices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Prices_PriceId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PriceId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Notifications");
        }
    }
}
