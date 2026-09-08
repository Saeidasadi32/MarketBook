using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketPrices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ListingId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TradingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OpenPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    HighPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    LowPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    LastPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    PreviousClosePrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    ReferencePrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    LowerLimit = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    UpperLimit = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketPrices_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_MarketPrices_Listing_TradingDate",
                table: "MarketPrices",
                columns: new[] { "ListingId", "TradingDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketPrices");
        }
    }
}
