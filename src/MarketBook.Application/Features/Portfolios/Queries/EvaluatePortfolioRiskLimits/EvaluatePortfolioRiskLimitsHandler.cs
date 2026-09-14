// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits;

/// <summary>
/// EN: Evaluates resolved limits using DOC-0043 as the sole risk-metric source and To as the persisted-policy as-of instant.
/// FA: Limitهای resolve‌شده را با DOC-0043 به‌عنوان تنها منبع Metric ریسک و To به‌عنوان لحظه As-Of Policy ذخیره‌شده ارزیابی می‌کند.
/// </summary>
public sealed class EvaluatePortfolioRiskLimitsHandler
    : IRequestHandler<EvaluatePortfolioRiskLimitsQuery, Result<EvaluatePortfolioRiskLimitsResponse>>
{
    private const string Maximum = "Maximum";
    private const string Minimum = "Minimum";
    private const string NotConfigured = "NotConfigured";
    private const string NotCalculable = "NotCalculable";
    private const string WithinLimit = "WithinLimit";
    private const string Breached = "Breached";

    private readonly ISender _sender;
    private readonly IPortfolioRiskPolicyRepository _riskPolicies;

    /// <summary>
    /// EN: Initializes the risk-limit evaluation handler.
    /// FA: Handler ارزیابی حدود ریسک را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    /// <param name="riskPolicies">EN: Persisted risk-policy repository. FA: Repository سیاست ریسک ذخیره‌شده.</param>
    public EvaluatePortfolioRiskLimitsHandler(
        ISender sender,
        IPortfolioRiskPolicyRepository riskPolicies)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(riskPolicies);

        _sender = sender;
        _riskPolicies = riskPolicies;
    }

    /// <summary>
    /// EN: Resolves request overrides over the policy effective as of To and evaluates them against compact risk-summary metrics.
    /// FA: Overrideهای Request را روی Policy معتبر در لحظه To resolve کرده و در برابر Metricهای خلاصه ریسک ارزیابی می‌کند.
    /// </summary>
    /// <param name="request">EN: Risk-limit request. FA: درخواست حدود ریسک.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Auditable risk-limit evaluation. FA: ارزیابی قابل ممیزی حدود ریسک.</returns>
    public async Task<Result<EvaluatePortfolioRiskLimitsResponse>> Handle(
        EvaluatePortfolioRiskLimitsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PortfolioRiskPolicy? effectivePolicy = null;

        if (PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) &&
            portfolioId is not null)
        {
            effectivePolicy =
                await _riskPolicies.GetEffectiveAsOfAsync(
                    portfolioId,
                    request.To,
                    cancellationToken);
        }

        ResolvedLimits limits =
            ResolveLimits(
                request,
                effectivePolicy);

        Result<GetPortfolioRiskSummaryResponse> summaryResult =
            await _sender.Send(
                new GetPortfolioRiskSummaryQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.ConfidenceLevel,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual),
                cancellationToken);

        if (summaryResult.IsFailure)
        {
            return Result<EvaluatePortfolioRiskLimitsResponse>.Fail(
                summaryResult.Error);
        }

        GetPortfolioRiskSummaryResponse summary =
            summaryResult.Value!;

        decimal? maximumDrawdownLossRatio =
            summary.Drawdown.MaximumDrawdown.HasValue
                ? Math.Max(-summary.Drawdown.MaximumDrawdown.Value, 0m)
                : null;

        List<PortfolioRiskLimitEvaluationResponse> rules =
        [
            EvaluateMaximum(
                "AnnualizedVolatility",
                limits.MaxAnnualizedVolatility,
                summary.RiskStatistics.AnnualizedVolatility),
            EvaluateMaximum(
                "ValueAtRiskReturn",
                limits.MaxValueAtRiskReturn,
                summary.ValueAtRisk.ValueAtRiskReturn),
            EvaluateMaximum(
                "ValueAtRiskAmountBase",
                limits.MaxValueAtRiskAmountBase,
                summary.ValueAtRisk.ValueAtRiskAmountBase),
            EvaluateMaximum(
                "MaximumDrawdownLossRatio",
                limits.MaxDrawdownLossRatio,
                maximumDrawdownLossRatio),
            EvaluateMaximum(
                "MaximumDrawdownAmountBase",
                limits.MaxDrawdownAmountBase,
                summary.Drawdown.MaximumDrawdownAmountBase),
            EvaluateMinimum(
                "SharpeRatio",
                limits.MinSharpeRatio,
                summary.RiskRatios.SharpeRatio),
            EvaluateMinimum(
                "SortinoRatio",
                limits.MinSortinoRatio,
                summary.RiskRatios.SortinoRatio)
        ];

        int configuredLimitCount =
            rules.Count(rule => rule.IsConfigured);

        int breachedLimitCount =
            rules.Count(rule => rule.Status == Breached);

        int notCalculableLimitCount =
            rules.Count(rule => rule.Status == NotCalculable);

        string overallStatus =
            ResolveOverallStatus(
                summary.IsComplete,
                configuredLimitCount,
                breachedLimitCount,
                notCalculableLimitCount);

        return Result<EvaluatePortfolioRiskLimitsResponse>.Success(
            new EvaluatePortfolioRiskLimitsResponse(
                summary.PortfolioId,
                summary.BaseCurrencyId,
                summary.From,
                summary.To,
                summary.Interval,
                request.To,
                limits.LimitSource,
                limits.PolicyId,
                limits.PolicyVersion,
                summary.IsComplete,
                overallStatus,
                configuredLimitCount,
                breachedLimitCount,
                notCalculableLimitCount,
                rules));
    }

    private static ResolvedLimits ResolveLimits(
        EvaluatePortfolioRiskLimitsQuery request,
        PortfolioRiskPolicy? effectivePolicy)
    {
        bool hasRequestLimit =
            request.MaxAnnualizedVolatility.HasValue ||
            request.MaxValueAtRiskReturn.HasValue ||
            request.MaxValueAtRiskAmountBase.HasValue ||
            request.MaxDrawdownLossRatio.HasValue ||
            request.MaxDrawdownAmountBase.HasValue ||
            request.MinSharpeRatio.HasValue ||
            request.MinSortinoRatio.HasValue;

        bool hasPersistedLimit =
            effectivePolicy is not null &&
            (
                effectivePolicy.MaxAnnualizedVolatility.HasValue ||
                effectivePolicy.MaxValueAtRiskReturn.HasValue ||
                effectivePolicy.MaxValueAtRiskAmountBase.HasValue ||
                effectivePolicy.MaxDrawdownLossRatio.HasValue ||
                effectivePolicy.MaxDrawdownAmountBase.HasValue ||
                effectivePolicy.MinSharpeRatio.HasValue ||
                effectivePolicy.MinSortinoRatio.HasValue
            );

        string limitSource =
            ResolveLimitSource(
                request,
                effectivePolicy,
                hasRequestLimit,
                hasPersistedLimit);

        return new ResolvedLimits(
            request.MaxAnnualizedVolatility ?? effectivePolicy?.MaxAnnualizedVolatility,
            request.MaxValueAtRiskReturn ?? effectivePolicy?.MaxValueAtRiskReturn,
            request.MaxValueAtRiskAmountBase ?? effectivePolicy?.MaxValueAtRiskAmountBase,
            request.MaxDrawdownLossRatio ?? effectivePolicy?.MaxDrawdownLossRatio,
            request.MaxDrawdownAmountBase ?? effectivePolicy?.MaxDrawdownAmountBase,
            request.MinSharpeRatio ?? effectivePolicy?.MinSharpeRatio,
            request.MinSortinoRatio ?? effectivePolicy?.MinSortinoRatio,
            limitSource,
            effectivePolicy?.Id.Value.ToString(),
            effectivePolicy?.PolicyVersion);
    }

    private static string ResolveLimitSource(
        EvaluatePortfolioRiskLimitsQuery request,
        PortfolioRiskPolicy? effectivePolicy,
        bool hasRequestLimit,
        bool hasPersistedLimit)
    {
        if (!hasRequestLimit && !hasPersistedLimit)
        {
            return "None";
        }

        if (!hasRequestLimit)
        {
            return "PersistedPolicy";
        }

        if (!hasPersistedLimit)
        {
            return "RequestOverride";
        }

        PortfolioRiskPolicy policy =
            effectivePolicy
            ?? throw new InvalidOperationException(
                "An effective persisted policy was expected while resolving the risk-limit source.");

        bool persistedFallbackUsed =
            (!request.MaxAnnualizedVolatility.HasValue && policy.MaxAnnualizedVolatility.HasValue) ||
            (!request.MaxValueAtRiskReturn.HasValue && policy.MaxValueAtRiskReturn.HasValue) ||
            (!request.MaxValueAtRiskAmountBase.HasValue && policy.MaxValueAtRiskAmountBase.HasValue) ||
            (!request.MaxDrawdownLossRatio.HasValue && policy.MaxDrawdownLossRatio.HasValue) ||
            (!request.MaxDrawdownAmountBase.HasValue && policy.MaxDrawdownAmountBase.HasValue) ||
            (!request.MinSharpeRatio.HasValue && policy.MinSharpeRatio.HasValue) ||
            (!request.MinSortinoRatio.HasValue && policy.MinSortinoRatio.HasValue);

        return persistedFallbackUsed
            ? "Mixed"
            : "RequestOverride";
    }

    private static PortfolioRiskLimitEvaluationResponse EvaluateMaximum(
        string code,
        decimal? limit,
        decimal? actual)
    {
        if (!limit.HasValue)
        {
            return CreateNotConfigured(
                code,
                Maximum);
        }

        if (!actual.HasValue)
        {
            return CreateNotCalculable(
                code,
                Maximum,
                limit.Value);
        }

        bool breached =
            actual.Value > limit.Value;

        return new PortfolioRiskLimitEvaluationResponse(
            code,
            Maximum,
            true,
            breached ? Breached : WithinLimit,
            limit.Value,
            actual.Value,
            breached
                ? actual.Value - limit.Value
                : null);
    }

    private static PortfolioRiskLimitEvaluationResponse EvaluateMinimum(
        string code,
        decimal? limit,
        decimal? actual)
    {
        if (!limit.HasValue)
        {
            return CreateNotConfigured(
                code,
                Minimum);
        }

        if (!actual.HasValue)
        {
            return CreateNotCalculable(
                code,
                Minimum,
                limit.Value);
        }

        bool breached =
            actual.Value < limit.Value;

        return new PortfolioRiskLimitEvaluationResponse(
            code,
            Minimum,
            true,
            breached ? Breached : WithinLimit,
            limit.Value,
            actual.Value,
            breached
                ? limit.Value - actual.Value
                : null);
    }

    private static PortfolioRiskLimitEvaluationResponse CreateNotConfigured(
        string code,
        string direction)
        => new(
            code,
            direction,
            false,
            NotConfigured,
            null,
            null,
            null);

    private static PortfolioRiskLimitEvaluationResponse CreateNotCalculable(
        string code,
        string direction,
        decimal limit)
        => new(
            code,
            direction,
            true,
            NotCalculable,
            limit,
            null,
            null);

    private static string ResolveOverallStatus(
        bool isComplete,
        int configuredLimitCount,
        int breachedLimitCount,
        int notCalculableLimitCount)
    {
        if (!isComplete)
        {
            return "Incomplete";
        }

        if (configuredLimitCount == 0)
        {
            return "NoLimitsConfigured";
        }

        if (breachedLimitCount > 0)
        {
            return Breached;
        }

        if (notCalculableLimitCount > 0)
        {
            return "Indeterminate";
        }

        return WithinLimit;
    }

    private sealed record ResolvedLimits(
        decimal? MaxAnnualizedVolatility,
        decimal? MaxValueAtRiskReturn,
        decimal? MaxValueAtRiskAmountBase,
        decimal? MaxDrawdownLossRatio,
        decimal? MaxDrawdownAmountBase,
        decimal? MinSharpeRatio,
        decimal? MinSortinoRatio,
        string LimitSource,
        string? PolicyId,
        int? PolicyVersion);
}
