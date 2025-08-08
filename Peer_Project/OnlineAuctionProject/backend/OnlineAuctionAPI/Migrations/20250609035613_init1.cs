using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionItemId1",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files",
                column: "AuctionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuctionItemId1",
                table: "Files",
                column: "AuctionItemId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId1",
                table: "Files",
                column: "AuctionItemId1",
                principalTable: "AuctionItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_AuctionItems_AuctionItemId1",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_AuctionItemId1",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AuctionItemId1",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AuctionItemId",
                table: "Files",
                column: "AuctionItemId",
                unique: true);
        }
    }
}
