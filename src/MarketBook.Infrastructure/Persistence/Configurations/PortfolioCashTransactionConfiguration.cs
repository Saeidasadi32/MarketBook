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
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>EN: Configures immutable portfolio cash-ledger persistence. FA: ماندگاری دفتر نقدی تغییرناپذیر پرتفوی را پیکربندی می‌کند.</summary>
public sealed class PortfolioCashTransactionConfiguration : IEntityTypeConfiguration<PortfolioCashTransaction>
{
    /// <summary>EN: Configures table, precision, FKs, and query indexes. FA: جدول، دقت، کلیدهای خارجی و ایندکس‌های Query را پیکربندی می‌کند.</summary>
    public void Configure(EntityTypeBuilder<PortfolioCashTransaction> builder)
    {
        builder.ToTable("PortfolioCashTransactions");
        builder.HasKey(item => item.Id);

        ValueConverter<PortfolioCashTransactionId, string> idConverter = new(
            id => id.Value.ToString(), value => PortfolioCashTransactionId.Parse(value));
        ValueConverter<PortfolioId, string> portfolioIdConverter = new(
            id => id.Value.ToString(), value => PortfolioId.Parse(value));
        ValueConverter<CurrencyId, string> currencyIdConverter = new(
            id => id.Value.ToString(), value => CurrencyId.Parse(value));

        builder.Property(item => item.Id).HasConversion(idConverter).HasMaxLength(26).IsUnicode(false).IsRequired();
        builder.Property(item => item.PortfolioId).HasConversion(portfolioIdConverter).HasMaxLength(26).IsUnicode(false).IsRequired();
        builder.Property(item => item.CurrencyId).HasConversion(currencyIdConverter).HasMaxLength(26).IsUnicode(false).IsRequired();
        builder.Property(item => item.Type).IsRequired();
        builder.Property(item => item.Amount).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.OccurredOn).IsRequired();
        builder.Property(item => item.ReferenceType).HasMaxLength(64);
        builder.Property(item => item.ReferenceId).HasMaxLength(128);
        builder.Property(item => item.Description).HasMaxLength(500);
        builder.Property(item => item.CreatedOn).IsRequired();
        builder.Ignore(item => item.IsCredit);
        builder.Ignore(item => item.SignedAmount);

        builder.HasOne<Portfolio>().WithMany().HasForeignKey(item => item.PortfolioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Currency>().WithMany().HasForeignKey(item => item.CurrencyId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new { item.PortfolioId, item.OccurredOn, item.Id })
            .HasDatabaseName("IX_PortfolioCashTransactions_Portfolio_OccurredOn_Id");
        builder.HasIndex(item => new { item.PortfolioId, item.CurrencyId, item.OccurredOn, item.Id })
            .HasDatabaseName("IX_PortfolioCashTransactions_Portfolio_Currency_OccurredOn_Id");

        // EN: Prevents a source transaction from being settled more than once.
        // FA: از تسویه بیش از یک‌باره یک تراکنش منبع جلوگیری می‌کند.
        builder.HasIndex(item => new { item.ReferenceType, item.ReferenceId })
            .IsUnique()
            .HasFilter("[ReferenceType] IS NOT NULL AND [ReferenceId] IS NOT NULL")
            .HasDatabaseName("UX_PortfolioCashTransactions_Reference");
    }
}
