// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the persistence mapping for the <see cref="Country"/> aggregate.
/// FA: نگاشت پایگاه داده Aggregate مربوط به <see cref="Country"/> را پیکربندی می‌کند.
/// </summary>
internal sealed class CountryConfiguration
    : IEntityTypeConfiguration<Country>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Countries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => CountryId.FromUlid(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasConversion(
                code => code.Value,
                value => new CountryCode(value))
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TimeZone)
            .HasConversion(
                id => id.Value,
                value => new TimeZoneId(value))
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Name);
    }
}
