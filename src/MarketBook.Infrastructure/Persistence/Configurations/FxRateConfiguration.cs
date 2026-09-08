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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures FX-rate persistence.
/// FA: نگاشت ماندگاری نرخ تبدیل ارز را پیکربندی می‌کند.
/// </summary>
public sealed class FxRateConfiguration : IEntityTypeConfiguration<FxRate>
{
    /// <summary>
    /// EN: Configures the FX-rate entity mapping.
    /// FA: نگاشت موجودیت نرخ ارز را پیکربندی می‌کند.
    /// </summary>
    /// <param name="builder">EN: Entity builder. FA: سازنده نگاشت موجودیت.</param>
    public void Configure(EntityTypeBuilder<FxRate> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("FxRates");
        builder.HasKey(item => item.Id);

        ValueConverter<FxRateId, string> idConverter = new(
            id => id.Value.ToString(),
            value => FxRateId.Parse(value));

        ValueConverter<CurrencyId, string> currencyIdConverter = new(
            id => id.Value.ToString(),
            value => CurrencyId.Parse(value));

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.BaseCurrencyId)
            .HasConversion(currencyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.QuoteCurrencyId)
            .HasConversion(currencyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.RateDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(item => item.Rate)
            .HasColumnType("decimal(38,10)")
            .IsRequired();

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(item => item.BaseCurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(item => item.QuoteCurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(item => new
            {
                item.BaseCurrencyId,
                item.QuoteCurrencyId,
                item.RateDate
            })
            .IsUnique()
            .HasDatabaseName("UX_FxRates_Base_Quote_Date");

        builder.HasIndex(item => new
            {
                item.BaseCurrencyId,
                item.QuoteCurrencyId,
                item.RateDate,
                item.CreatedOn
            })
            .HasDatabaseName("IX_FxRates_Latest");
    }
}
