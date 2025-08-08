using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AuctionTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AuctionDeleteRequests_AuctionItemId",
                table: "AuctionDeleteRequests",
                column: "AuctionItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionDeleteRequests_AuctionItems_AuctionItemId",
                table: "AuctionDeleteRequests",
                column: "AuctionItemId",
                principalTable: "AuctionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionDeleteRequests_AuctionItems_AuctionItemId",
                table: "AuctionDeleteRequests");

            migrationBuilder.DropIndex(
                name: "IX_AuctionDeleteRequests_AuctionItemId",
                table: "AuctionDeleteRequests");
        }
    }
}
