using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyTradeStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTradeStatistics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ListingId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TradingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    TradeCount = table.Column<int>(type: "int", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    TradeValue = table.Column<decimal>(type: "decimal(38,2)", nullable: false),
                    MarketCapitalization = table.Column<decimal>(type: "decimal(38,2)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTradeStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTradeStatistics_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_DailyTradeStatistics_Listing_TradingDate",
                table: "DailyTradeStatistics",
                columns: new[] { "ListingId", "TradingDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTradeStatistics");
        }
    }
}
