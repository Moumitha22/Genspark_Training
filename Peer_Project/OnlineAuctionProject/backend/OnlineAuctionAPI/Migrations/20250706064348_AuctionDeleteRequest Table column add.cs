using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AuctionDeleteRequestTablecolumnadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AuctionDeleteRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AuctionDeleteRequests");
        }
    }
}
