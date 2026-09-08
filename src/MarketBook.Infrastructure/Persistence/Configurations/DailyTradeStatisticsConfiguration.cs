// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.MarketData.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures daily trade-statistics persistence.
/// FA: نگاشت ماندگاری آمار معاملات روزانه را پیکربندی می‌کند.
/// </summary>
public sealed class DailyTradeStatisticsConfiguration
    : IEntityTypeConfiguration<DailyTradeStatistics>
{
    /// <summary>
    /// EN: Configures entity mapping.
    /// FA: نگاشت موجودیت را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<DailyTradeStatistics> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DailyTradeStatistics");
        builder.HasKey(item => item.Id);

        ValueConverter<DailyTradeStatisticsId, string> idConverter = new(
            id => id.Value.ToString(),
            value => DailyTradeStatisticsId.Parse(value));

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<ListingId, string> listingIdConverter = new(
            id => id.Value.ToString(),
            value => ListingId.Parse(value));

        builder.Property(item => item.ListingId)
            .HasConversion(listingIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.TradingDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(item => item.Volume)
            .IsRequired();

        builder.Property(item => item.TradeCount)
            .IsRequired();

        builder.Property(item => item.AveragePrice)
            .HasColumnType("decimal(38,6)")
            .IsRequired();

        builder.Property(item => item.TradeValue)
            .HasColumnType("decimal(38,2)")
            .IsRequired();

        builder.Property(item => item.MarketCapitalization)
            .HasColumnType("decimal(38,2)")
            .IsRequired();

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.Property(item => item.UpdatedOn)
            .IsRequired();

        builder.HasIndex(item => new { item.ListingId, item.TradingDate })
            .IsUnique()
            .HasDatabaseName("UX_DailyTradeStatistics_Listing_TradingDate");

        builder.HasOne<Listing>()
            .WithMany()
            .HasForeignKey(item => item.ListingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
