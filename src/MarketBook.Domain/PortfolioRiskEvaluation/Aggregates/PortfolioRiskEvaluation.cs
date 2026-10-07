// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskEvaluation.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Entities;
using MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;

namespace MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;

/// <summary>
/// EN: Represents an immutable historical snapshot of one portfolio risk-limit evaluation.
/// FA: Snapshot ØªØ§Ø±ÛŒØ®ÛŒ Ùˆ Ù…Ø§Ù†Ø¯Ú¯Ø§Ø± Ø§Ø² ÛŒÚ© Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ Ø­Ø¯ÙˆØ¯ Ø±ÛŒØ³Ú© Ù¾Ø±ØªÙÙˆÛŒ.
/// </summary>
public sealed class PortfolioRiskEvaluation : AggregateRoot<PortfolioRiskEvaluationId>
{
    private readonly List<PortfolioRiskEvaluationRule> _rules = [];

    /// <summary>
    /// EN: Creates a persisted risk evaluation snapshot.
    /// FA: ÛŒÚ© Snapshot Ù…Ø§Ù†Ø¯Ú¯Ø§Ø± Ø§Ø² Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ Ø±ÛŒØ³Ú© Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
    /// </summary>
    public PortfolioRiskEvaluation(
        PortfolioRiskEvaluationId id,
        PortfolioId portfolioId,
        string baseCurrencyId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual,
        DateTimeOffset policyAsOf,
        string limitSource,
        PortfolioRiskPolicyId? policyId,
        int? policyVersion,
        bool isComplete,
        string overallStatus,
        int configuredLimitCount,
        int breachedLimitCount,
        int notCalculableLimitCount,
        IEnumerable<PortfolioRiskEvaluationRule> rules)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(portfolioId);
        ArgumentNullException.ThrowIfNull(rules);

        Guard.AgainstNullOrWhiteSpace(baseCurrencyId, nameof(baseCurrencyId));
        Guard.AgainstNullOrWhiteSpace(interval, nameof(interval));
        Guard.AgainstNullOrWhiteSpace(limitSource, nameof(limitSource));
        Guard.AgainstNullOrWhiteSpace(overallStatus, nameof(overallStatus));

        if (from > to)
        {
            throw new DomainException(
                new Error(
                    "PortfolioRiskEvaluation.InvalidPeriod",
                    "From must be less than or equal to To."));
        }

        if (confidenceLevel <= 0m || confidenceLevel >= 1m)
        {
            throw new DomainException(
                new Error(
                    "PortfolioRiskEvaluation.InvalidConfidenceLevel",
                    "Confidence level must be greater than zero and less than one."));
        }

        if (configuredLimitCount < 0 ||
            breachedLimitCount < 0 ||
            notCalculableLimitCount < 0)
        {
            throw new DomainException(
                new Error(
                    "PortfolioRiskEvaluation.InvalidCounts",
                    "Evaluation counts cannot be negative."));
        }

        if (policyVersion.HasValue && policyVersion.Value < 1)
        {
            throw new DomainException(
                new Error(
                    "PortfolioRiskEvaluation.InvalidPolicyVersion",
                    "Policy version must be positive when supplied."));
        }

        PortfolioId = portfolioId;
        BaseCurrencyId = baseCurrencyId.Trim();
        From = from;
        To = to;
        Interval = interval.Trim();
        ConfidenceLevel = confidenceLevel;
        RiskFreeRateAnnual = riskFreeRateAnnual;
        MinimumAcceptableReturnAnnual = minimumAcceptableReturnAnnual;
        PolicyAsOf = policyAsOf;
        LimitSource = limitSource.Trim();
        PolicyId = policyId;
        PolicyVersion = policyVersion;
        IsComplete = isComplete;
        OverallStatus = overallStatus.Trim();
        ConfiguredLimitCount = configuredLimitCount;
        BreachedLimitCount = breachedLimitCount;
        NotCalculableLimitCount = notCalculableLimitCount;
        EvaluatedOn = DateTimeOffset.UtcNow;

        _rules.AddRange(rules);
    }

    private PortfolioRiskEvaluation()
    {
        PortfolioId = default!;
        BaseCurrencyId = string.Empty;
        Interval = string.Empty;
        LimitSource = string.Empty;
        OverallStatus = string.Empty;
    }

    /// <summary>EN: Portfolio identifier. FA: Ø´Ù†Ø§Ø³Ù‡ Ù¾Ø±ØªÙÙˆÛŒ.</summary>
    public PortfolioId PortfolioId { get; private set; }

    /// <summary>
    /// EN: Base-currency identifier captured as snapshot data.
    /// FA: Ø´Ù†Ø§Ø³Ù‡ Ø§Ø±Ø² Ù¾Ø§ÛŒÙ‡ Ø«Ø¨Øªâ€ŒØ´Ø¯Ù‡ Ø¨Ù‡ Ø¹Ù†ÙˆØ§Ù† Ø¯Ø§Ø¯Ù‡ Snapshot.
    /// </summary>
    public string BaseCurrencyId { get; private set; }

    /// <summary>EN: Evaluation period start. FA: Ø§Ø¨ØªØ¯Ø§ÛŒ Ø¯ÙˆØ±Ù‡ Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ.</summary>
    public DateTimeOffset From { get; private set; }

    /// <summary>EN: Evaluation period end. FA: Ø§Ù†ØªÙ‡Ø§ÛŒ Ø¯ÙˆØ±Ù‡ Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ.</summary>
    public DateTimeOffset To { get; private set; }

    /// <summary>EN: Sampling interval. FA: ÙØ§ØµÙ„Ù‡ Ù†Ù…ÙˆÙ†Ù‡â€ŒØ¨Ø±Ø¯Ø§Ø±ÛŒ.</summary>
    public string Interval { get; private set; }

    /// <summary>EN: VaR/CVaR confidence level. FA: Ø³Ø·Ø­ Ø§Ø·Ù…ÛŒÙ†Ø§Ù† VaR/CVaR.</summary>
    public decimal ConfidenceLevel { get; private set; }

    /// <summary>EN: Annual risk-free rate used by Sharpe. FA: Ù†Ø±Ø® Ø¨Ø¯ÙˆÙ† Ø±ÛŒØ³Ú© Ø³Ø§Ù„Ø§Ù†Ù‡ Ø§Ø³ØªÙØ§Ø¯Ù‡â€ŒØ´Ø¯Ù‡ Ø¯Ø± Sharpe.</summary>
    public decimal RiskFreeRateAnnual { get; private set; }

    /// <summary>EN: Annual MAR used by Sortino. FA: MAR Ø³Ø§Ù„Ø§Ù†Ù‡ Ø§Ø³ØªÙØ§Ø¯Ù‡â€ŒØ´Ø¯Ù‡ Ø¯Ø± Sortino.</summary>
    public decimal MinimumAcceptableReturnAnnual { get; private set; }

    /// <summary>EN: Instant used for historical persisted-policy selection. FA: Ù„Ø­Ø¸Ù‡ As-Of Ø§Ù†ØªØ®Ø§Ø¨ Policy.</summary>
    public DateTimeOffset PolicyAsOf { get; private set; }

    /// <summary>EN: None, PersistedPolicy, RequestOverride, or Mixed. FA: Ù…Ù†Ø¨Ø¹ LimitÙ‡Ø§ÛŒ Ù†Ù‡Ø§ÛŒÛŒ.</summary>
    public string LimitSource { get; private set; }

    /// <summary>EN: Persisted policy identifier when applicable. FA: Ø´Ù†Ø§Ø³Ù‡ Policy Ø°Ø®ÛŒØ±Ù‡â€ŒØ´Ø¯Ù‡ Ø¯Ø± ØµÙˆØ±Øª ÙˆØ¬ÙˆØ¯.</summary>
    public PortfolioRiskPolicyId? PolicyId { get; private set; }

    /// <summary>EN: Persisted policy business version when applicable. FA: Ù†Ø³Ø®Ù‡ Ú©Ø³Ø¨â€ŒÙˆÚ©Ø§Ø±ÛŒ Policy Ø¯Ø± ØµÙˆØ±Øª ÙˆØ¬ÙˆØ¯.</summary>
    public int? PolicyVersion { get; private set; }

    /// <summary>EN: Source risk summary completeness. FA: Ú©Ø§Ù…Ù„â€ŒØ¨ÙˆØ¯Ù† Ø®Ù„Ø§ØµÙ‡ Ø±ÛŒØ³Ú© Ù…Ù†Ø¨Ø¹.</summary>
    public bool IsComplete { get; private set; }

    /// <summary>EN: Overall evaluation status. FA: ÙˆØ¶Ø¹ÛŒØª Ú©Ù„ÛŒ Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ.</summary>
    public string OverallStatus { get; private set; }

    /// <summary>EN: Configured limit count. FA: ØªØ¹Ø¯Ø§Ø¯ LimitÙ‡Ø§ÛŒ Ù¾ÛŒÚ©Ø±Ø¨Ù†Ø¯ÛŒâ€ŒØ´Ø¯Ù‡.</summary>
    public int ConfiguredLimitCount { get; private set; }

    /// <summary>EN: Breached limit count. FA: ØªØ¹Ø¯Ø§Ø¯ LimitÙ‡Ø§ÛŒ Ù†Ù‚Ø¶â€ŒØ´Ø¯Ù‡.</summary>
    public int BreachedLimitCount { get; private set; }

    /// <summary>EN: Not-calculable limit count. FA: ØªØ¹Ø¯Ø§Ø¯ LimitÙ‡Ø§ÛŒ ØºÛŒØ±Ù‚Ø§Ø¨Ù„ Ù…Ø­Ø§Ø³Ø¨Ù‡.</summary>
    public int NotCalculableLimitCount { get; private set; }

    /// <summary>EN: UTC instant when the snapshot was persisted. FA: Ø²Ù…Ø§Ù† UTC Ø«Ø¨Øª Snapshot.</summary>
    public DateTimeOffset EvaluatedOn { get; private set; }

    /// <summary>EN: Ordered resolved rule snapshots. FA: SnapshotÙ‡Ø§ÛŒ Ù…Ø±ØªØ¨ RuleÙ‡Ø§ÛŒ Ø­Ù„â€ŒØ´Ø¯Ù‡.</summary>
    public IReadOnlyCollection<PortfolioRiskEvaluationRule> Rules => _rules.AsReadOnly();
}