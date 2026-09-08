using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioCashLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioCashTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PortfolioId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CurrencyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioCashTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioCashTransactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PortfolioCashTransactions_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioCashTransactions_CurrencyId",
                table: "PortfolioCashTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioCashTransactions_Portfolio_Currency_OccurredOn_Id",
                table: "PortfolioCashTransactions",
                columns: new[] { "PortfolioId", "CurrencyId", "OccurredOn", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioCashTransactions_Portfolio_OccurredOn_Id",
                table: "PortfolioCashTransactions",
                columns: new[] { "PortfolioId", "OccurredOn", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioCashTransactions");
        }
    }
}
