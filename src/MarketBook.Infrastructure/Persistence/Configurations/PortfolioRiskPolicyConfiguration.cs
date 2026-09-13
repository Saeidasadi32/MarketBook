// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MarketBook.Domain.PortfolioRiskPolicy.Enums;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>EN: Configures persisted portfolio risk policies. FA: نگاشت Policyهای ریسک پرتفوی را پیکربندی می‌کند.</summary>
public sealed class PortfolioRiskPolicyConfiguration : IEntityTypeConfiguration<PortfolioRiskPolicy>
{
    /// <summary>EN: Configures EF mapping. FA: نگاشت EF را پیکربندی می‌کند.</summary>
    public void Configure(EntityTypeBuilder<PortfolioRiskPolicy> builder)
    {
        ValueConverter<PortfolioRiskPolicyId, string> idConverter = new(
            id => id.Value.ToString(),
            value => PortfolioRiskPolicyId.Parse(value));

        ValueConverter<PortfolioId, string> portfolioIdConverter = new(
            id => id.Value.ToString(),
            value => PortfolioId.Parse(value));

        builder.ToTable("PortfolioRiskPolicies");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasConversion(idConverter).HasMaxLength(26).IsUnicode(false);
        builder.Property(item => item.PortfolioId).HasConversion(portfolioIdConverter).HasMaxLength(26).IsUnicode(false);
        builder.Property(item => item.PolicyVersion).IsRequired();
        builder.Property(item => item.EffectiveFrom).IsRequired();
        builder.Property(item => item.EffectiveTo).IsRequired(false);
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(item => item.MaxAnnualizedVolatility).HasPrecision(18, 8);
        builder.Property(item => item.MaxValueAtRiskReturn).HasPrecision(18, 8);
        builder.Property(item => item.MaxValueAtRiskAmountBase).HasPrecision(24, 8);
        builder.Property(item => item.MaxDrawdownLossRatio).HasPrecision(18, 8);
        builder.Property(item => item.MaxDrawdownAmountBase).HasPrecision(24, 8);
        builder.Property(item => item.MinSharpeRatio).HasPrecision(18, 8);
        builder.Property(item => item.MinSortinoRatio).HasPrecision(18, 8);
        builder.Property(item => item.CreatedOn).IsRequired();
        builder.Property(item => item.UpdatedOn).IsRequired();

        builder.HasOne<Portfolio>().WithMany().HasForeignKey(item => item.PortfolioId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new { item.PortfolioId, item.PolicyVersion })
            .IsUnique()
            .HasDatabaseName("UX_PortfolioRiskPolicies_Portfolio_Version");

        builder.HasIndex(item => item.PortfolioId)
            .HasFilter("[Status] = 'Active'")
            .IsUnique()
            .HasDatabaseName("UX_PortfolioRiskPolicies_OneActive");
    }
}
