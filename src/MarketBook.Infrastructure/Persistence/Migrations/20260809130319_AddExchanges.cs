using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CountryId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_Code",
                table: "Exchanges",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_CountryId",
                table: "Exchanges",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exchanges");
        }
    }
}
