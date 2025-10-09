using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropFinderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLimitAttribute_DiscountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListerUsageCount",
                table: "DiscountCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxListerLimit",
                table: "DiscountCodes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListerUsageCount",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "MaxListerLimit",
                table: "DiscountCodes");
        }
    }
}
