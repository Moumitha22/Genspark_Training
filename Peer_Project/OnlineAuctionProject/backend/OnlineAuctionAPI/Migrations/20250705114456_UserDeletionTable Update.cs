using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserDeletionTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "DeletedUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "DeletedUsers");
        }
    }
}
