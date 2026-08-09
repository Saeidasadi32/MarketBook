// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the Exchange aggregate for Entity Framework Core.
/// FA: تنظیمات ماندگاری Aggregate بورس را برای Entity Framework Core انجام می‌دهد.
/// </summary>
internal sealed class ExchangeConfiguration
    : IEntityTypeConfiguration<Exchange>
{
    /// <summary>
    /// EN: Configures the Exchange entity.
    /// FA: موجودیت Exchange را پیکربندی می‌کند.
    /// </summary>
    /// <param name="builder">
    /// EN: Entity type builder.
    /// FA: سازنده نوع موجودیت.
    /// </param>
    public void Configure(EntityTypeBuilder<Exchange> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ValueConverter<ExchangeId, string> idConverter =
            new(
                id => id.Value.ToString(),
                value => ExchangeId.Parse(value));

        ValueConverter<CountryId, string> countryIdConverter =
            new(
                id => id.Value.ToString(),
                value => CountryId.Parse(value));

        ValueConverter<ExchangeCode, string> codeConverter =
            new(
                code => code.Value,
                value => new ExchangeCode(value));

        builder.ToTable("Exchanges");

        builder.HasKey(exchange => exchange.Id);

        builder.Property(exchange => exchange.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .ValueGeneratedNever();

        builder.Property(exchange => exchange.CountryId)
            .HasConversion(countryIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(exchange => exchange.Code)
            .HasConversion(codeConverter)
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(exchange => exchange.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(exchange => exchange.CreatedOn)
            .IsRequired();

        builder.Property(exchange => exchange.IsActive)
            .IsRequired();

        builder.HasIndex(exchange => exchange.Code)
            .IsUnique();

        builder.HasIndex(exchange => exchange.CountryId);
    }
}
