using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class MakeExchangeCountryOptional : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "CountryId",
            table: "Exchanges",
            type: "varchar(26)",
            unicode: false,
            maxLength: 26,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(26)",
            oldUnicode: false,
            oldMaxLength: 26);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
    """
            DELETE FROM Exchanges
            WHERE CountryId IS NULL;
            """);

        migrationBuilder.AlterColumn<string>(
            name: "CountryId",
            table: "Exchanges",
            type: "varchar(26)",
            unicode: false,
            maxLength: 26,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(26)",
            oldUnicode: false,
            oldMaxLength: 26,
            oldNullable: true);
    }
}
