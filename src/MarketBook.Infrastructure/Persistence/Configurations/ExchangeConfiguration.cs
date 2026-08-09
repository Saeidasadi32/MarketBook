// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
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

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the Exchange aggregate for Entity Framework Core.
/// FA: Aggregate بورس را برای Entity Framework Core پیکربندی می‌کند.
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

        builder.ToTable("Exchanges");

        builder.HasKey(exchange => exchange.Id);

        builder.Property(exchange => exchange.Id)
            .HasConversion(
                id => id.Value,
                value => ExchangeId.FromUlid(value))
            .HasMaxLength(26)
            .ValueGeneratedNever();

        builder.Property(exchange => exchange.CountryId)
            .HasConversion(
                id => id.Value,
                value => CountryId.FromUlid(value))
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(exchange => exchange.Code)
            .HasConversion(
                code => code.Value,
                value => new ExchangeCode(value))
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
