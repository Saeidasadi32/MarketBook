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
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures immutable portfolio transaction persistence.
/// FA: ماندگاری تراکنش تغییرناپذیر پرتفوی را پیکربندی می‌کند.
/// </summary>
public sealed class PortfolioTransactionConfiguration
    : IEntityTypeConfiguration<PortfolioTransaction>
{
    public void Configure(EntityTypeBuilder<PortfolioTransaction> builder)
    {
        builder.ToTable("PortfolioTransactions");
        builder.HasKey(item => item.Id);

        ValueConverter<PortfolioTransactionId, string> idConverter = new(
            id => id.Value.ToString(),
            value => PortfolioTransactionId.Parse(value));

        ValueConverter<PortfolioId, string> portfolioIdConverter = new(
            id => id.Value.ToString(),
            value => PortfolioId.Parse(value));

        ValueConverter<ListingId, string> listingIdConverter = new(
            id => id.Value.ToString(),
            value => ListingId.Parse(value));

        ValueConverter<CurrencyId, string> currencyIdConverter = new(
            id => id.Value.ToString(),
            value => CurrencyId.Parse(value));

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.PortfolioId)
            .HasConversion(portfolioIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.ListingId)
            .HasConversion(listingIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.CurrencyId)
            .HasConversion(currencyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.Type).IsRequired();

        builder.Property(item => item.Quantity)
            .HasPrecision(38, 10)
            .IsRequired();

        builder.Property(item => item.Price)
            .HasPrecision(38, 10)
            .IsRequired();

        builder.Property(item => item.Commission).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.Tax).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.ExchangeFee).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.BrokerFee).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.ClearingFee).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.OtherFees).HasPrecision(38, 10).IsRequired();
        builder.Property(item => item.ExecutedOn).IsRequired();
        builder.Property(item => item.CreatedOn).IsRequired();

        builder.HasOne<Portfolio>()
            .WithMany()
            .HasForeignKey(item => item.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Listing>()
            .WithMany()
            .HasForeignKey(item => item.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(item => item.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new { item.PortfolioId, item.ExecutedOn, item.Id })
            .HasDatabaseName("IX_PortfolioTransactions_Portfolio_ExecutedOn_Id");

        builder.HasIndex(item => new { item.PortfolioId, item.ListingId, item.ExecutedOn, item.Id })
            .HasDatabaseName("IX_PortfolioTransactions_Portfolio_Listing_ExecutedOn_Id");
    }
}
