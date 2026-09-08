using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIntradayPriceTick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntradayPriceTicks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ListingId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TradingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38,6)", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntradayPriceTicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntradayPriceTicks_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntradayPriceTicks_Listing_TradingDate_OccurredAt_Sequence",
                table: "IntradayPriceTicks",
                columns: new[] { "ListingId", "TradingDate", "OccurredAt", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "UX_IntradayPriceTicks_Listing_TradingDate_Sequence",
                table: "IntradayPriceTicks",
                columns: new[] { "ListingId", "TradingDate", "SequenceNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntradayPriceTicks");
        }
    }
}
