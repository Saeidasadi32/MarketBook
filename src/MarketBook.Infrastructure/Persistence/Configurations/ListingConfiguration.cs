// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures Listing persistence and relational invariants.
/// FA: نگاشت ذخیره‌سازی و قیود رابطه‌ای Listing را پیکربندی می‌کند.
/// </summary>
public sealed class ListingConfiguration
    : IEntityTypeConfiguration<Listing>
{
    /// <summary>
    /// EN: Configures the entity mapping.
    /// FA: نگاشت موجودیت را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Listings");
        builder.HasKey(listing => listing.Id);

        ValueConverter<ListingId, string> idConverter = new(
            id => id.Value.ToString(),
            value => ListingId.Parse(value));

        builder.Property(listing => listing.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<InstrumentId, string> instrumentIdConverter = new(
            id => id.Value.ToString(),
            value => InstrumentId.Parse(value));

        builder.Property(listing => listing.InstrumentId)
            .HasConversion(instrumentIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<VenueId, string> venueIdConverter = new(
            id => id.Value.ToString(),
            value => VenueId.Parse(value));

        builder.Property(listing => listing.VenueId)
            .HasConversion(venueIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<CurrencyId, string> currencyIdConverter = new(
            id => id.Value.ToString(),
            value => CurrencyId.Parse(value));

        builder.Property(listing => listing.QuoteCurrencyId)
            .HasConversion(currencyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<TradingSymbol, string> tradingSymbolConverter = new(
            symbol => symbol.Value,
            value => new TradingSymbol(value));

        builder.Property(listing => listing.TradingSymbol)
            .HasConversion(tradingSymbolConverter)
            .HasMaxLength(30)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(listing => listing.TickSize)
            .HasColumnType("decimal(38,28)")
            .IsRequired();

        builder.Property(listing => listing.PricePrecision)
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(listing => listing.IsPrimary)
            .IsRequired();

        builder.Property(listing => listing.CreatedOn)
            .IsRequired();

        builder.Property(listing => listing.IsActive)
            .IsRequired();

        builder.Property(listing => listing.ActivatedOn);

        builder.Property(listing => listing.DeactivatedOn);

        builder.HasIndex(
                listing => new
                {
                    listing.VenueId,
                    listing.TradingSymbol
                })
            .IsUnique()
            .HasDatabaseName("UX_Listings_Venue_TradingSymbol");

        builder.HasIndex(listing => listing.InstrumentId)
            .IsUnique()
            .HasDatabaseName("UX_Listings_Instrument_Primary")
            .HasFilter("[IsPrimary] = 1");

        builder.HasIndex(listing => listing.QuoteCurrencyId);

        builder.HasOne<Instrument>()
            .WithMany()
            .HasForeignKey(listing => listing.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(listing => listing.VenueId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(listing => listing.QuoteCurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
