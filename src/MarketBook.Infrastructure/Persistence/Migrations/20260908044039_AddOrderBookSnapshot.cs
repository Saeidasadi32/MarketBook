using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderBookSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderBookSnapshots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ListingId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TradingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CapturedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBookSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderBookSnapshots_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderBookSnapshotLevels",
                columns: table => new
                {
                    Level = table.Column<int>(type: "int", nullable: false),
                    OrderBookSnapshotId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: true),
                    BidVolume = table.Column<long>(type: "bigint", nullable: true),
                    BidOrderCount = table.Column<int>(type: "int", nullable: true),
                    AskPrice = table.Column<decimal>(type: "decimal(38,6)", nullable: true),
                    AskVolume = table.Column<long>(type: "bigint", nullable: true),
                    AskOrderCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBookSnapshotLevels", x => new { x.OrderBookSnapshotId, x.Level });
                    table.ForeignKey(
                        name: "FK_OrderBookSnapshotLevels_OrderBookSnapshots_OrderBookSnapshotId",
                        column: x => x.OrderBookSnapshotId,
                        principalTable: "OrderBookSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderBookSnapshots_Listing_TradingDate_CapturedAt_Sequence",
                table: "OrderBookSnapshots",
                columns: new[] { "ListingId", "TradingDate", "CapturedAt", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "UX_OrderBookSnapshots_Listing_TradingDate_Sequence",
                table: "OrderBookSnapshots",
                columns: new[] { "ListingId", "TradingDate", "SequenceNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderBookSnapshotLevels");

            migrationBuilder.DropTable(
                name: "OrderBookSnapshots");
        }
    }
}
