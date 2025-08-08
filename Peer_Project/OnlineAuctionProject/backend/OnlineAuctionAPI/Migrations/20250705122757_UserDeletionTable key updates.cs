using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserDeletionTablekeyupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeletedUsers_UserId",
                table: "DeletedUsers");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedUsers_Email",
                table: "DeletedUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeletedUsers_UserId",
                table: "DeletedUsers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeletedUsers_Email",
                table: "DeletedUsers");

            migrationBuilder.DropIndex(
                name: "IX_DeletedUsers_UserId",
                table: "DeletedUsers");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedUsers_UserId",
                table: "DeletedUsers",
                column: "UserId");
        }
    }
}
