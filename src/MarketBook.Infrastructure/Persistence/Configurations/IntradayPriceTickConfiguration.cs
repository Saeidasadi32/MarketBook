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
/// EN: Configures intraday price-tick persistence.
/// FA: نگاشت ماندگاری Tickهای قیمت درون‌روزی را پیکربندی می‌کند.
/// </summary>
public sealed class IntradayPriceTickConfiguration
    : IEntityTypeConfiguration<IntradayPriceTick>
{
    /// <summary>
    /// EN: Configures entity mapping.
    /// FA: نگاشت موجودیت را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<IntradayPriceTick> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("IntradayPriceTicks");
        builder.HasKey(item => item.Id);

        ValueConverter<IntradayPriceTickId, string> idConverter = new(
            id => id.Value.ToString(),
            value => IntradayPriceTickId.Parse(value));

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

        builder.Property(item => item.OccurredAt)
            .IsRequired();

        builder.Property(item => item.SequenceNumber)
            .IsRequired();

        builder.Property(item => item.Price)
            .HasColumnType("decimal(38,6)")
            .IsRequired();

        builder.Property(item => item.Volume)
            .IsRequired();

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.Ignore(item => item.TradeValue);

        builder.HasIndex(
                item => new
                {
                    item.ListingId,
                    item.TradingDate,
                    item.SequenceNumber
                })
            .IsUnique()
            .HasDatabaseName(
                "UX_IntradayPriceTicks_Listing_TradingDate_Sequence");

        builder.HasIndex(
                item => new
                {
                    item.ListingId,
                    item.TradingDate,
                    item.OccurredAt,
                    item.SequenceNumber
                })
            .HasDatabaseName(
                "IX_IntradayPriceTicks_Listing_TradingDate_OccurredAt_Sequence");

        builder.HasOne<Listing>()
            .WithMany()
            .HasForeignKey(item => item.ListingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
