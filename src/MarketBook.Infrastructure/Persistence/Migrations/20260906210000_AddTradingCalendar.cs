// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Migrations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBook.Infrastructure.Persistence.Migrations;

/// <summary>
/// EN: Adds configurable yearly trading calendars, weekly sessions, and date exceptions.
/// FA: تقویم‌های معاملاتی سالانه قابل تنظیم، Sessionهای هفتگی و استثناهای تاریخی را اضافه می‌کند.
/// </summary>
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260906210000_AddTradingCalendar")]
public sealed class AddTradingCalendar : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TradingCalendars",
            columns: table => new
            {
                Id = table.Column<string>(
                    type: "varchar(26)",
                    unicode: false,
                    maxLength: 26,
                    nullable: false),
                MarketId = table.Column<string>(
                    type: "varchar(26)",
                    unicode: false,
                    maxLength: 26,
                    nullable: false),
                Year = table.Column<int>(
                    type: "int",
                    nullable: false),
                WeekendDays = table.Column<int>(
                    type: "int",
                    nullable: false),
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
                    "PK_TradingCalendars",
                    calendar => calendar.Id);

                table.ForeignKey(
                    name: "FK_TradingCalendars_Markets_MarketId",
                    column: calendar => calendar.MarketId,
                    principalTable: "Markets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TradingCalendarDateExceptions",
            columns: table => new
            {
                Id = table.Column<int>(
                    type: "int",
                    nullable: false)
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"),
                TradingCalendarId = table.Column<string>(
                    type: "varchar(26)",
                    nullable: false),
                Date = table.Column<DateOnly>(
                    type: "date",
                    nullable: false),
                IsHoliday = table.Column<bool>(
                    type: "bit",
                    nullable: false),
                IsHalfDay = table.Column<bool>(
                    type: "bit",
                    nullable: false),
                Description = table.Column<string>(
                    type: "nvarchar(500)",
                    maxLength: 500,
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_TradingCalendarDateExceptions",
                    item => item.Id);

                table.ForeignKey(
                    name: "FK_TradingCalendarDateExceptions_TradingCalendars_TradingCalendarId",
                    column: item => item.TradingCalendarId,
                    principalTable: "TradingCalendars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TradingCalendarSessions",
            columns: table => new
            {
                Id = table.Column<int>(
                    type: "int",
                    nullable: false)
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"),
                TradingCalendarId = table.Column<string>(
                    type: "varchar(26)",
                    nullable: false),
                DayOfWeek = table.Column<int>(
                    type: "int",
                    nullable: false),
                OpensAt = table.Column<TimeOnly>(
                    type: "time",
                    nullable: false),
                ClosesAt = table.Column<TimeOnly>(
                    type: "time",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_TradingCalendarSessions",
                    session => session.Id);

                table.ForeignKey(
                    name: "FK_TradingCalendarSessions_TradingCalendars_TradingCalendarId",
                    column: session => session.TradingCalendarId,
                    principalTable: "TradingCalendars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "UX_TradingCalendars_Market_Year",
            table: "TradingCalendars",
            columns: new[] { "MarketId", "Year" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_TradingCalendarDateExceptions_Calendar_Date",
            table: "TradingCalendarDateExceptions",
            columns: new[] { "TradingCalendarId", "Date" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_TradingCalendarSessions_Calendar_Day",
            table: "TradingCalendarSessions",
            columns: new[] { "TradingCalendarId", "DayOfWeek" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TradingCalendarDateExceptions");

        migrationBuilder.DropTable(
            name: "TradingCalendarSessions");

        migrationBuilder.DropTable(
            name: "TradingCalendars");
    }
}
