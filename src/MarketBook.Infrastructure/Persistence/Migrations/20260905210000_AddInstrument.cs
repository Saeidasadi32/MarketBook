using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInstrument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<string>(
                        type: "varchar(26)",
                        unicode: false,
                        maxLength: 26,
                        nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),
                    AssetClass = table.Column<string>(
                        type: "varchar(100)",
                        unicode: false,
                        maxLength: 100,
                        nullable: false),
                    Type = table.Column<int>(
                        type: "int",
                        nullable: false),
                    Category = table.Column<string>(
                        type: "varchar(100)",
                        unicode: false,
                        maxLength: 100,
                        nullable: false),
                    Isin = table.Column<string>(
                        type: "varchar(12)",
                        unicode: false,
                        maxLength: 12,
                        nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: false),
                    IsActive = table.Column<bool>(
                        type: "bit",
                        nullable: false),
                    Version = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Instruments",
                        instrument => instrument.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Isin",
                table: "Instruments",
                column: "Isin",
                unique: true,
                filter: "[Isin] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instruments");
        }
    }
}
