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
using MarketBook.Domain.MarketData.Entities;
using MarketBook.Domain.MarketData.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>EN: Configures order-book snapshot persistence. FA: نگاشت ماندگاری Snapshotهای دفتر سفارشات را پیکربندی می‌کند.</summary>
public sealed class OrderBookSnapshotConfiguration
    : IEntityTypeConfiguration<OrderBookSnapshot>
{
    /// <summary>EN: Configures entity mapping. FA: نگاشت موجودیت را پیکربندی می‌کند.</summary>
    public void Configure(EntityTypeBuilder<OrderBookSnapshot> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("OrderBookSnapshots");
        builder.HasKey(item => item.Id);

        ValueConverter<OrderBookSnapshotId, string> idConverter = new(
            id => id.Value.ToString(),
            value => OrderBookSnapshotId.Parse(value));

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

        builder.Property(item => item.CapturedAt)
            .IsRequired();

        builder.Property(item => item.SequenceNumber)
            .IsRequired();

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.HasIndex(
                item => new
                {
                    item.ListingId,
                    item.TradingDate,
                    item.SequenceNumber
                })
            .IsUnique()
            .HasDatabaseName(
                "UX_OrderBookSnapshots_Listing_TradingDate_Sequence");

        builder.HasIndex(
                item => new
                {
                    item.ListingId,
                    item.TradingDate,
                    item.CapturedAt,
                    item.SequenceNumber
                })
            .HasDatabaseName(
                "IX_OrderBookSnapshots_Listing_TradingDate_CapturedAt_Sequence");

        builder.HasOne<Listing>()
            .WithMany()
            .HasForeignKey(item => item.ListingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.OwnsMany(
            item => item.Levels,
            levels => ConfigureLevels(levels, idConverter));

        builder.Navigation(item => item.Levels)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureLevels(
        OwnedNavigationBuilder<OrderBookSnapshot, OrderBookSnapshotLevel> levels,
        ValueConverter<OrderBookSnapshotId, string> idConverter)
    {
        levels.ToTable("OrderBookSnapshotLevels");
        levels.WithOwner()
            .HasForeignKey("OrderBookSnapshotId");

        levels.Property<OrderBookSnapshotId>("OrderBookSnapshotId")
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

levels.Property(item => item.Level)
    .ValueGeneratedNever()
    .IsRequired();

        levels.HasKey(
            "OrderBookSnapshotId",
            nameof(OrderBookSnapshotLevel.Level));

        levels.Property(item => item.BidPrice)
            .HasColumnType("decimal(38,6)");

        levels.Property(item => item.BidVolume);
        levels.Property(item => item.BidOrderCount);

        levels.Property(item => item.AskPrice)
            .HasColumnType("decimal(38,6)");

        levels.Property(item => item.AskVolume);
        levels.Property(item => item.AskOrderCount);
    }
}
