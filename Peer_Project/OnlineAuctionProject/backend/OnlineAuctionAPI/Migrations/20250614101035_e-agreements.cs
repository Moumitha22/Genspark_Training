using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class eagreements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VirtualWallets_UserId",
                table: "VirtualWallets");

            migrationBuilder.CreateTable(
                name: "EAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiddingId = table.Column<Guid>(type: "uuid", nullable: false),
                    File = table.Column<byte[]>(type: "bytea", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EAgreements_AuctionItems_AuctionItemId",
                        column: x => x.AuctionItemId,
                        principalTable: "AuctionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EAgreements_BidItems_BiddingId",
                        column: x => x.BiddingId,
                        principalTable: "BidItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VirtualWallets_UserId",
                table: "VirtualWallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EAgreements_AuctionItemId",
                table: "EAgreements",
                column: "AuctionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_EAgreements_BiddingId",
                table: "EAgreements",
                column: "BiddingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAgreements");

            migrationBuilder.DropIndex(
                name: "IX_VirtualWallets_UserId",
                table: "VirtualWallets");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualWallets_UserId",
                table: "VirtualWallets",
                column: "UserId");
        }
    }
}
