// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures Instrument persistence.
/// FA: نگاشت ذخیره‌سازی Instrument را پیکربندی می‌کند.
/// </summary>
public sealed class InstrumentConfiguration
    : IEntityTypeConfiguration<Instrument>
{
    /// <summary>
    /// EN: Configures the entity mapping.
    /// FA: نگاشت موجودیت را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Instruments");
        builder.HasKey(instrument => instrument.Id);

        ValueConverter<InstrumentId, string> idConverter = new(
            id => id.Value.ToString(),
            value => InstrumentId.Parse(value));

        builder.Property(instrument => instrument.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<InstrumentName, string> nameConverter = new(
            name => name.Value,
            value => new InstrumentName(value));

        builder.Property(instrument => instrument.Name)
            .HasConversion(nameConverter)
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();

        ValueConverter<AssetClass, string> assetClassConverter = new(
            assetClass => assetClass.Value,
            value => new AssetClass(value));

        builder.Property(instrument => instrument.AssetClass)
            .HasConversion(assetClassConverter)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(instrument => instrument.Type)
            .HasColumnType("int")
            .IsRequired();

        ValueConverter<InstrumentCategory, string> categoryConverter = new(
            category => category.Value,
            value => new InstrumentCategory(value));

        builder.Property(instrument => instrument.Category)
            .HasConversion(categoryConverter)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<Isin?, string?> isinConverter = new(
            isin => isin == null ? null : isin.Value,
            value => string.IsNullOrWhiteSpace(value) ? null : new Isin(value));

        builder.Property(instrument => instrument.Isin)
            .HasConversion(isinConverter)
            .HasMaxLength(12)
            .IsUnicode(false);

        builder.HasIndex(instrument => instrument.Isin)
            .IsUnique()
            .HasFilter("[Isin] IS NOT NULL");

        builder.Property(instrument => instrument.CreatedOn)
            .IsRequired();

        builder.Property(instrument => instrument.IsActive)
            .IsRequired();

        builder.Ignore(instrument => instrument.Industry);
        builder.Ignore(instrument => instrument.Sector);
        builder.Ignore(instrument => instrument.CorporateAliases);
    }
}
