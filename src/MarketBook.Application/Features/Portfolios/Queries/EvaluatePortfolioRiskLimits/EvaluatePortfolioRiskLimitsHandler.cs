// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits;

/// <summary>
/// EN: Evaluates request-scoped limits using DOC-0043 as the sole risk-metric source.
/// FA: حدود request-scoped را با استفاده از DOC-0043 به‌عنوان تنها منبع Metricهای ریسک ارزیابی می‌کند.
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

    /// <summary>
    /// EN: Initializes the risk-limit evaluation handler.
    /// FA: Handler ارزیابی حدود ریسک را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public EvaluatePortfolioRiskLimitsHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Compares configured limits with compact risk-summary metrics.
    /// FA: Limitهای پیکربندی‌شده را با Metricهای خلاصه فشرده ریسک مقایسه می‌کند.
    /// </summary>
    /// <param name="request">EN: Risk-limit request. FA: درخواست حدود ریسک.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Auditable risk-limit evaluation. FA: ارزیابی قابل ممیزی حدود ریسک.</returns>
    public async Task<Result<EvaluatePortfolioRiskLimitsResponse>> Handle(
        EvaluatePortfolioRiskLimitsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

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
                request.MaxAnnualizedVolatility,
                summary.RiskStatistics.AnnualizedVolatility),
            EvaluateMaximum(
                "ValueAtRiskReturn",
                request.MaxValueAtRiskReturn,
                summary.ValueAtRisk.ValueAtRiskReturn),
            EvaluateMaximum(
                "ValueAtRiskAmountBase",
                request.MaxValueAtRiskAmountBase,
                summary.ValueAtRisk.ValueAtRiskAmountBase),
            EvaluateMaximum(
                "MaximumDrawdownLossRatio",
                request.MaxDrawdownLossRatio,
                maximumDrawdownLossRatio),
            EvaluateMaximum(
                "MaximumDrawdownAmountBase",
                request.MaxDrawdownAmountBase,
                summary.Drawdown.MaximumDrawdownAmountBase),
            EvaluateMinimum(
                "SharpeRatio",
                request.MinSharpeRatio,
                summary.RiskRatios.SharpeRatio),
            EvaluateMinimum(
                "SortinoRatio",
                request.MinSortinoRatio,
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
                summary.IsComplete,
                overallStatus,
                configuredLimitCount,
                breachedLimitCount,
                notCalculableLimitCount,
                rules));
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
}
