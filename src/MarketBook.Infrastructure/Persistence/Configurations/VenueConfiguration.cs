// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the Venue aggregate for Entity Framework Core persistence.
/// FA: تنظیمات Aggregate مربوط به بستر معاملاتی را برای ذخیره‌سازی توسط Entity Framework Core انجام می‌دهد.
/// </summary>
public sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    /// <summary>
    /// EN: Configures the Venue entity and its database mapping.
    /// FA: موجودیت Venue و نگاشت آن به پایگاه داده را پیکربندی می‌کند.
    /// </summary>
    /// <param name="builder">
    /// EN: Entity type builder used to configure Venue.
    /// FA: Builder مربوط به پیکربندی موجودیت Venue.
    /// </param>
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Venues");

        builder.HasKey(venue => venue.Id);

        ValueConverter<VenueId, string> venueIdConverter =
            new(
                venueId => venueId.Value.ToString(),
                value => VenueId.Parse(value));

        builder.Property(venue => venue.Id)
            .HasConversion(venueIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<VenueCode, string> venueCodeConverter =
            new(
                code => code.Value,
                value => new VenueCode(value));

        builder.Property(venue => venue.Code)
            .HasConversion(venueCodeConverter)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(venue => venue.Code)
            .IsUnique();

        ValueConverter<MarketId, string> marketIdConverter =
            new(
                marketId => marketId.Value.ToString(),
                value => MarketId.Parse(value));

        builder.Property(venue => venue.MarketId)
            .HasConversion(marketIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.HasOne<Market>()
            .WithMany()
            .HasForeignKey(venue => venue.MarketId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(venue => venue.Name)
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();

        builder.Property(venue => venue.Type)
            .IsRequired();

        builder.Property(venue => venue.CreatedOn)
            .IsRequired();

        builder.Property(venue => venue.IsActive)
            .IsRequired();
    }
}
