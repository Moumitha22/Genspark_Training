using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class fileauctioncascaderel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionItems_Users_WinnerId",
                table: "AuctionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BidItems_AuctionItems_AuctionItemId1",
                table: "BidItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_BidItems_AuctionItemId1",
                table: "BidItems");

            migrationBuilder.DropIndex(
                name: "IX_AuctionItems_WinnerId",
                table: "AuctionItems");

            migrationBuilder.DropColumn(
                name: "AuctionItemId1",
                table: "BidItems");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionItems_WinnerId",
                table: "AuctionItems",
                column: "WinnerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionItems_BidItems_WinnerId",
                table: "AuctionItems",
                column: "WinnerId",
                principalTable: "BidItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId",
                table: "Files",
                column: "AuctionItemId",
                principalTable: "AuctionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionItems_BidItems_WinnerId",
                table: "AuctionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_AuctionItems_WinnerId",
                table: "AuctionItems");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionItemId1",
                table: "BidItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BidItems_AuctionItemId1",
                table: "BidItems",
                column: "AuctionItemId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionItems_WinnerId",
                table: "AuctionItems",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionItems_Users_WinnerId",
                table: "AuctionItems",
                column: "WinnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BidItems_AuctionItems_AuctionItemId1",
                table: "BidItems",
                column: "AuctionItemId1",
                principalTable: "AuctionItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId",
                table: "Files",
                column: "AuctionItemId",
                principalTable: "AuctionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
