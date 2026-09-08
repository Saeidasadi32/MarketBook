using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioTransactionLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PortfolioId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ListingId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CurrencyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    ExchangeFee = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    BrokerFee = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    ClearingFee = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    OtherFees = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    ExecutedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioTransactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PortfolioTransactions_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PortfolioTransactions_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_CurrencyId",
                table: "PortfolioTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_ListingId",
                table: "PortfolioTransactions",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_Portfolio_ExecutedOn_Id",
                table: "PortfolioTransactions",
                columns: new[] { "PortfolioId", "ExecutedOn", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_Portfolio_Listing_ExecutedOn_Id",
                table: "PortfolioTransactions",
                columns: new[] { "PortfolioId", "ListingId", "ExecutedOn", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioTransactions");
        }
    }
}
