using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioRiskPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioRiskPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PortfolioId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PolicyVersion = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxAnnualizedVolatility = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    MaxValueAtRiskReturn = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    MaxValueAtRiskAmountBase = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    MaxDrawdownLossRatio = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    MaxDrawdownAmountBase = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    MinSharpeRatio = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    MinSortinoRatio = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioRiskPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioRiskPolicies_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_PortfolioRiskPolicies_OneActive",
                table: "PortfolioRiskPolicies",
                column: "PortfolioId",
                unique: true,
                filter: "[Status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "UX_PortfolioRiskPolicies_Portfolio_Version",
                table: "PortfolioRiskPolicies",
                columns: new[] { "PortfolioId", "PolicyVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioRiskPolicies");
        }
    }
}
