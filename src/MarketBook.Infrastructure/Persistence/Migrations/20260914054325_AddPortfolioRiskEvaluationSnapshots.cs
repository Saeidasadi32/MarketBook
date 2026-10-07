using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioRiskEvaluationSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioRiskEvaluations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PortfolioId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    BaseCurrencyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    From = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Interval = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    RiskFreeRateAnnual = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    MinimumAcceptableReturnAnnual = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    PolicyAsOf = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LimitSource = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    PolicyId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    PolicyVersion = table.Column<int>(type: "int", nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    OverallStatus = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    ConfiguredLimitCount = table.Column<int>(type: "int", nullable: false),
                    BreachedLimitCount = table.Column<int>(type: "int", nullable: false),
                    NotCalculableLimitCount = table.Column<int>(type: "int", nullable: false),
                    EvaluatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioRiskEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioRiskEvaluations_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioRiskEvaluationRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Direction = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    IsConfigured = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    Actual = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    BreachAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    PortfolioRiskEvaluationId = table.Column<string>(type: "varchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioRiskEvaluationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioRiskEvaluationRules_PortfolioRiskEvaluations_PortfolioRiskEvaluationId",
                        column: x => x.PortfolioRiskEvaluationId,
                        principalTable: "PortfolioRiskEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioRiskEvaluationRules_Evaluation_Code",
                table: "PortfolioRiskEvaluationRules",
                columns: new[] { "PortfolioRiskEvaluationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioRiskEvaluations_Portfolio_EvaluatedOn",
                table: "PortfolioRiskEvaluations",
                columns: new[] { "PortfolioId", "EvaluatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioRiskEvaluations_Portfolio_PolicyAsOf",
                table: "PortfolioRiskEvaluations",
                columns: new[] { "PortfolioId", "PolicyAsOf" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioRiskEvaluationRules");

            migrationBuilder.DropTable(
                name: "PortfolioRiskEvaluations");
        }
    }
}
