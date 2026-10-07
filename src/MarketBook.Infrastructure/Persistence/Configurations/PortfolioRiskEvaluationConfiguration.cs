using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.PortfolioRiskEvaluation.Entities;
using MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

public sealed class PortfolioRiskEvaluationConfiguration
    : IEntityTypeConfiguration<PortfolioRiskEvaluation>
{
    public void Configure(EntityTypeBuilder<PortfolioRiskEvaluation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ValueConverter<PortfolioRiskEvaluationId, string> idConverter = new(
            id => id.Value.ToString(),
            value => PortfolioRiskEvaluationId.Parse(value));

        ValueConverter<PortfolioId, string> portfolioIdConverter = new(
            id => id.Value.ToString(),
            value => PortfolioId.Parse(value));

        ValueConverter<PortfolioRiskPolicyId?, string?> policyIdConverter = new(
            id => id == null ? null : id.Value.ToString(),
            value => string.IsNullOrWhiteSpace(value)
                ? null
                : PortfolioRiskPolicyId.Parse(value));

        builder.ToTable("PortfolioRiskEvaluations");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .ValueGeneratedNever();

        builder.Property(item => item.PortfolioId)
            .HasConversion(portfolioIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.BaseCurrencyId)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.From)
            .IsRequired();

        builder.Property(item => item.To)
            .IsRequired();

        builder.Property(item => item.Interval)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.ConfidenceLevel)
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(item => item.RiskFreeRateAnnual)
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(item => item.MinimumAcceptableReturnAnnual)
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(item => item.PolicyAsOf)
            .IsRequired();

        builder.Property(item => item.LimitSource)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.PolicyId)
            .HasConversion(policyIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false);

        builder.Property(item => item.PolicyVersion);

        builder.Property(item => item.IsComplete)
            .IsRequired();

        builder.Property(item => item.OverallStatus)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.ConfiguredLimitCount)
            .IsRequired();

        builder.Property(item => item.BreachedLimitCount)
            .IsRequired();

        builder.Property(item => item.NotCalculableLimitCount)
            .IsRequired();

        builder.Property(item => item.EvaluatedOn)
            .IsRequired();

        builder.HasOne<Portfolio>()
            .WithMany()
            .HasForeignKey(item => item.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new { item.PortfolioId, item.EvaluatedOn })
            .HasDatabaseName(
                "IX_PortfolioRiskEvaluations_Portfolio_EvaluatedOn");

        builder.HasIndex(item => new { item.PortfolioId, item.PolicyAsOf })
            .HasDatabaseName(
                "IX_PortfolioRiskEvaluations_Portfolio_PolicyAsOf");

        builder.OwnsMany(
            item => item.Rules,
            owned =>
            {
                owned.ToTable("PortfolioRiskEvaluationRules");

                owned.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                owned.HasKey("Id");

                owned.WithOwner()
                    .HasForeignKey("PortfolioRiskEvaluationId");

                owned.Property(item => item.Code)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .IsRequired();

                owned.Property(item => item.Direction)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsRequired();

                owned.Property(item => item.IsConfigured)
                    .IsRequired();

                owned.Property(item => item.Status)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsRequired();

                owned.Property(item => item.Limit)
                    .HasPrecision(24, 8);

                owned.Property(item => item.Actual)
                    .HasPrecision(24, 8);

                owned.Property(item => item.BreachAmount)
                    .HasPrecision(24, 8);

                owned.HasIndex(
                        "PortfolioRiskEvaluationId",
                        nameof(PortfolioRiskEvaluationRule.Code))
                    .IsUnique()
                    .HasDatabaseName(
                        "IX_PortfolioRiskEvaluationRules_Evaluation_Code");
            });

        builder.Navigation(item => item.Rules)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
