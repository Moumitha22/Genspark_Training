using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropFinderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddListerDetailsToContactLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListerEmail",
                table: "ContactLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ListerName",
                table: "ContactLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ListerPhoneNumber",
                table: "ContactLogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListerEmail",
                table: "ContactLogs");

            migrationBuilder.DropColumn(
                name: "ListerName",
                table: "ContactLogs");

            migrationBuilder.DropColumn(
                name: "ListerPhoneNumber",
                table: "ContactLogs");
        }
    }
}
