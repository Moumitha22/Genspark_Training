using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files",
                column: "AuctionItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files",
                column: "AuctionItemId",
                unique: true);
        }
    }
}
