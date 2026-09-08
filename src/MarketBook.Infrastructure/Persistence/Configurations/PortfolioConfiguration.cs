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
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures Portfolio persistence.
/// FA: نگاشت ماندگاری Portfolio را پیکربندی می‌کند.
/// </summary>
public sealed class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    /// <summary>
    /// EN: Configures entity mapping.
    /// FA: نگاشت موجودیت را پیکربندی می‌کند.
    /// </summary>
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Portfolios");
        builder.HasKey(item => item.Id);

        ValueConverter<PortfolioId, string> idConverter = new(
            id => id.Value.ToString(),
            value => PortfolioId.Parse(value));

        ValueConverter<InvestorId, string> investorIdConverter = new(
            id => id.Value.ToString(),
            value => InvestorId.Parse(value));

        ValueConverter<PortfolioName, string> nameConverter = new(
            name => name.Value,
            value => new PortfolioName(value));

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.InvestorId)
            .HasConversion(investorIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.Name)
            .HasConversion(nameConverter)
            .HasMaxLength(PortfolioName.MaxLength)
            .IsUnicode()
            .IsRequired();

        ValueConverter<CurrencyId?, string?> baseCurrencyIdConverter = new(
            id => id == null ? null : id.Value.ToString(),
            value => value == null ? null : CurrencyId.Parse(value));

        builder.Property(item => item.BaseCurrencyId)
            .HasConversion(baseCurrencyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.Property(item => item.IsActive)
            .IsRequired();

        builder.Ignore(item => item.Positions);
        builder.Ignore(item => item.Events);

        builder.HasOne<Investor>()
            .WithMany()
            .HasForeignKey(item => item.InvestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(item => item.BaseCurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(item => item.BaseCurrencyId)
            .HasDatabaseName("IX_Portfolios_BaseCurrencyId");

        builder.HasIndex(item => new
            {
                item.InvestorId,
                item.Name
            })
            .IsUnique()
            .HasDatabaseName("UX_Portfolios_InvestorId_Name");

        builder.HasIndex(item => item.InvestorId)
            .HasDatabaseName("IX_Portfolios_InvestorId");
    }
}
