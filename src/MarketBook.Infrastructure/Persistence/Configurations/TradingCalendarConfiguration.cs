// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures trading-calendar persistence, sessions, and date exceptions.
/// FA: ماندگاری تقویم معاملاتی، Sessionها و استثناهای تاریخی را پیکربندی می‌کند.
/// </summary>
public sealed class TradingCalendarConfiguration
    : IEntityTypeConfiguration<TradingCalendar>
{
    /// <summary>
    /// EN: Configures the EF Core model.
    /// FA: مدل EF Core را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<TradingCalendar> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("TradingCalendars");
        builder.HasKey(calendar => calendar.Id);

        ValueConverter<TradingCalendarId, string> idConverter = new(
            id => id.Value.ToString(),
            value => TradingCalendarId.Parse(value));

        builder.Property(calendar => calendar.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<MarketId, string> marketIdConverter = new(
            id => id.Value.ToString(),
            value => MarketId.Parse(value));

        builder.Property(calendar => calendar.MarketId)
            .HasConversion(marketIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(calendar => calendar.Year)
            .IsRequired();

        builder.Property(calendar => calendar.WeekendDays)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(calendar => calendar.CreatedOn)
            .IsRequired();

        builder.Property(calendar => calendar.IsActive)
            .IsRequired();

        builder.HasIndex(
                calendar => new
                {
                    calendar.MarketId,
                    calendar.Year
                })
            .IsUnique()
            .HasDatabaseName("UX_TradingCalendars_Market_Year");

        builder.HasOne<Market>()
            .WithMany()
            .HasForeignKey(calendar => calendar.MarketId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.OwnsMany(
            calendar => calendar.Sessions,
            sessions =>
            {
                sessions.ToTable("TradingCalendarSessions");
                sessions.WithOwner()
                    .HasForeignKey("TradingCalendarId");

                sessions.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                sessions.HasKey("Id");

                sessions.Property(session => session.DayOfWeek)
                    .HasConversion<int>()
                    .IsRequired();

                sessions.Property(session => session.OpensAt)
                    .HasColumnType("time")
                    .IsRequired();

                sessions.Property(session => session.ClosesAt)
                    .HasColumnType("time")
                    .IsRequired();

                sessions.HasIndex(
                        "TradingCalendarId",
                        nameof(MarketBook.Domain.Calendar.Entities.TradingSession.DayOfWeek))
                    .IsUnique()
                    .HasDatabaseName("UX_TradingCalendarSessions_Calendar_Day");
            });

        builder.OwnsMany(
            calendar => calendar.DateExceptions,
            exceptions =>
            {
                exceptions.ToTable("TradingCalendarDateExceptions");
                exceptions.WithOwner()
                    .HasForeignKey("TradingCalendarId");

                exceptions.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                exceptions.HasKey("Id");

                exceptions.Property(item => item.Date)
                    .HasColumnType("date")
                    .IsRequired();

                exceptions.Property(item => item.IsHoliday)
                    .IsRequired();

                exceptions.Property(item => item.IsHalfDay)
                    .IsRequired();

                exceptions.Property(item => item.Description)
                    .HasMaxLength(500)
                    .IsUnicode();

                exceptions.HasIndex(
                        "TradingCalendarId",
                        nameof(MarketBook.Domain.Calendar.Entities.CalendarDateException.Date))
                    .IsUnique()
                    .HasDatabaseName("UX_TradingCalendarDateExceptions_Calendar_Date");
            });
    }
}
