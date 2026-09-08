using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioBaseCurrencyAndFxRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseCurrencyId",
                table: "Portfolios",
                type: "varchar(26)",
                unicode: false,
                maxLength: 26,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FxRates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    BaseCurrencyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    QuoteCurrencyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    RateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(38,10)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FxRates_Currencies_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FxRates_Currencies_QuoteCurrencyId",
                        column: x => x.QuoteCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_BaseCurrencyId",
                table: "Portfolios",
                column: "BaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_FxRates_Latest",
                table: "FxRates",
                columns: new[] { "BaseCurrencyId", "QuoteCurrencyId", "RateDate", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_FxRates_QuoteCurrencyId",
                table: "FxRates",
                column: "QuoteCurrencyId");

            migrationBuilder.CreateIndex(
                name: "UX_FxRates_Base_Quote_Date",
                table: "FxRates",
                columns: new[] { "BaseCurrencyId", "QuoteCurrencyId", "RateDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Currencies_BaseCurrencyId",
                table: "Portfolios",
                column: "BaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Currencies_BaseCurrencyId",
                table: "Portfolios");

            migrationBuilder.DropTable(
                name: "FxRates");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_BaseCurrencyId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "BaseCurrencyId",
                table: "Portfolios");
        }
    }
}
