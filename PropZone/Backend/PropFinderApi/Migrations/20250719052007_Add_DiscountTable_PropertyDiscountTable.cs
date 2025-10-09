using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropFinderApi.Migrations
{
    /// <inheritdoc />
    public partial class Add_DiscountTable_PropertyDiscountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ToDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCodeOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeOfProperty = table.Column<string>(type: "text", nullable: true),
                    PurposeOfListing = table.Column<string>(type: "text", nullable: true),
                    MinPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountCodeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountCodeOptions_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDiscountCodes",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountCodeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDiscountCode", x => new { x.PropertyId, x.DiscountCodeId });
                    table.ForeignKey(
                        name: "FK_PropertyDiscountCodes_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyDiscountCodes_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodeOptions_DiscountCodeId",
                table: "DiscountCodeOptions",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDiscountCodes_DiscountCodeId",
                table: "PropertyDiscountCodes",
                column: "DiscountCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountCodeOptions");

            migrationBuilder.DropTable(
                name: "PropertyDiscountCodes");

            migrationBuilder.DropTable(
                name: "DiscountCodes");
        }
    }
}
