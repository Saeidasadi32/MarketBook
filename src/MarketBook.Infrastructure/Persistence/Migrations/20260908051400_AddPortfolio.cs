using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    InvestorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Investors_InvestorId",
                        column: x => x.InvestorId,
                        principalTable: "Investors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_InvestorId",
                table: "Portfolios",
                column: "InvestorId");

            migrationBuilder.CreateIndex(
                name: "UX_Portfolios_InvestorId_Name",
                table: "Portfolios",
                columns: new[] { "InvestorId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Portfolios");
        }
    }
}
