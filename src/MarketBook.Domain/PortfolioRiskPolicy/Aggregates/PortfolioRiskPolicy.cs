// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskPolicy.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Enums;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;

namespace MarketBook.Domain.PortfolioRiskPolicy.Aggregates;

/// <summary>
/// EN: Represents one effective-dated version of a portfolio risk policy.
/// FA: یک نسخه تاریخ‌دار از سیاست ریسک پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed class PortfolioRiskPolicy : AggregateRoot<PortfolioRiskPolicyId>
{
    /// <summary>
    /// EN: Creates a persisted risk-policy version.
    /// FA: یک نسخه ذخیره‌شونده سیاست ریسک ایجاد می‌کند.
    /// </summary>
    public PortfolioRiskPolicy(
        PortfolioRiskPolicyId id,
        PortfolioId portfolioId,
        int policyVersion,
        DateTimeOffset effectiveFrom,
        RiskPolicyStatus status,
        decimal? maxAnnualizedVolatility,
        decimal? maxValueAtRiskReturn,
        decimal? maxValueAtRiskAmountBase,
        decimal? maxDrawdownLossRatio,
        decimal? maxDrawdownAmountBase,
        decimal? minSharpeRatio,
        decimal? minSortinoRatio)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(portfolioId);

        if (policyVersion < 1)
        {
            throw new DomainException(
                new Error("PortfolioRiskPolicy.InvalidVersion", "Policy version must be positive."));
        }

        ValidateMaximum(maxAnnualizedVolatility, nameof(maxAnnualizedVolatility));
        ValidateMaximum(maxValueAtRiskReturn, nameof(maxValueAtRiskReturn));
        ValidateMaximum(maxValueAtRiskAmountBase, nameof(maxValueAtRiskAmountBase));
        ValidateMaximum(maxDrawdownLossRatio, nameof(maxDrawdownLossRatio));
        ValidateMaximum(maxDrawdownAmountBase, nameof(maxDrawdownAmountBase));

        PortfolioId = portfolioId;
        PolicyVersion = policyVersion;
        EffectiveFrom = effectiveFrom;
        Status = status;
        MaxAnnualizedVolatility = maxAnnualizedVolatility;
        MaxValueAtRiskReturn = maxValueAtRiskReturn;
        MaxValueAtRiskAmountBase = maxValueAtRiskAmountBase;
        MaxDrawdownLossRatio = maxDrawdownLossRatio;
        MaxDrawdownAmountBase = maxDrawdownAmountBase;
        MinSharpeRatio = minSharpeRatio;
        MinSortinoRatio = minSortinoRatio;
        CreatedOn = DateTimeOffset.UtcNow;
        UpdatedOn = CreatedOn;
    }

    private PortfolioRiskPolicy()
    {
        PortfolioId = default!;
    }

    /// <summary>EN: Portfolio identifier. FA: شناسه پرتفوی.</summary>
    public PortfolioId PortfolioId { get; private set; }

    /// <summary>EN: Business policy version. FA: نسخه کسب‌وکاری Policy.</summary>
    public int PolicyVersion { get; private set; }

    /// <summary>EN: Effective start instant. FA: لحظه شروع اعتبار.</summary>
    public DateTimeOffset EffectiveFrom { get; private set; }

    /// <summary>EN: Effective end instant when archived. FA: لحظه پایان اعتبار در صورت بایگانی.</summary>
    public DateTimeOffset? EffectiveTo { get; private set; }

    /// <summary>EN: Policy lifecycle status. FA: وضعیت چرخه عمر Policy.</summary>
    public RiskPolicyStatus Status { get; private set; }

    /// <summary>EN: Maximum annualized volatility. FA: حداکثر نوسان سالانه‌شده.</summary>
    public decimal? MaxAnnualizedVolatility { get; private set; }

    /// <summary>EN: Maximum VaR return loss ratio. FA: حداکثر نسبت زیان VaR.</summary>
    public decimal? MaxValueAtRiskReturn { get; private set; }

    /// <summary>EN: Maximum VaR amount in base currency. FA: حداکثر مبلغ VaR در ارز پایه.</summary>
    public decimal? MaxValueAtRiskAmountBase { get; private set; }

    /// <summary>EN: Maximum positive drawdown-loss ratio. FA: حداکثر بزرگی مثبت Drawdown.</summary>
    public decimal? MaxDrawdownLossRatio { get; private set; }

    /// <summary>EN: Maximum monetary drawdown. FA: حداکثر Drawdown مبلغی.</summary>
    public decimal? MaxDrawdownAmountBase { get; private set; }

    /// <summary>EN: Minimum Sharpe ratio. FA: حداقل نسبت Sharpe.</summary>
    public decimal? MinSharpeRatio { get; private set; }

    /// <summary>EN: Minimum Sortino ratio. FA: حداقل نسبت Sortino.</summary>
    public decimal? MinSortinoRatio { get; private set; }

    /// <summary>EN: Creation instant. FA: لحظه ایجاد.</summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>EN: Last update instant. FA: آخرین لحظه تغییر.</summary>
    public DateTimeOffset UpdatedOn { get; private set; }

    /// <summary>EN: Activates this version. FA: این نسخه را فعال می‌کند.</summary>
    public void Activate(DateTimeOffset effectiveFrom)
    {
        EffectiveFrom = effectiveFrom;
        EffectiveTo = null;
        Status = RiskPolicyStatus.Active;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>EN: Archives this version. FA: این نسخه را بایگانی می‌کند.</summary>
    public void Archive(DateTimeOffset effectiveTo)
    {
        EffectiveTo = effectiveTo;
        Status = RiskPolicyStatus.Archived;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    private static void ValidateMaximum(decimal? value, string name)
    {
        if (value.HasValue && value.Value <= 0m)
        {
            throw new DomainException(
                new Error("PortfolioRiskPolicy.InvalidMaximumLimit", $"{name} must be greater than zero."));
        }
    }
}
