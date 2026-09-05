using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<string>(
                        type: "varchar(26)",
                        unicode: false,
                        maxLength: 26,
                        nullable: false),
                    InstrumentId = table.Column<string>(
                        type: "varchar(26)",
                        unicode: false,
                        maxLength: 26,
                        nullable: false),
                    VenueId = table.Column<string>(
                        type: "varchar(26)",
                        unicode: false,
                        maxLength: 26,
                        nullable: false),
                    TradingSymbol = table.Column<string>(
                        type: "varchar(30)",
                        unicode: false,
                        maxLength: 30,
                        nullable: false),
                    QuoteCurrencyId = table.Column<string>(
                        type: "varchar(26)",
                        unicode: false,
                        maxLength: 26,
                        nullable: false),
                    TickSize = table.Column<decimal>(
                        type: "decimal(38,28)",
                        nullable: false),
                    PricePrecision = table.Column<byte>(
                        type: "tinyint",
                        nullable: false),
                    IsPrimary = table.Column<bool>(
                        type: "bit",
                        nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false),
                    IsActive = table.Column<bool>(
                        type: "bit",
                        nullable: false),
                    ActivatedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: true),
                    DeactivatedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: true),
                    Version = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", listing => listing.Id);

                    table.ForeignKey(
                        name: "FK_Listings_Currencies_QuoteCurrencyId",
                        column: listing => listing.QuoteCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Listings_Instruments_InstrumentId",
                        column: listing => listing.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Listings_Venues_VenueId",
                        column: listing => listing.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_QuoteCurrencyId",
                table: "Listings",
                column: "QuoteCurrencyId");

            migrationBuilder.CreateIndex(
                name: "UX_Listings_Instrument_Primary",
                table: "Listings",
                column: "InstrumentId",
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_Listings_Venue_TradingSymbol",
                table: "Listings",
                columns: new[] { "VenueId", "TradingSymbol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Listings");
        }
    }
}
