// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios;

/// <summary>
/// EN: Composes DOC-0033 risk statistics into configurable Sharpe and Sortino ratios.
/// FA: آمار ریسک DOC-0033 را به نسبت‌های Sharpe و Sortino قابل‌تنظیم تبدیل می‌کند.
/// </summary>
public sealed class GetPortfolioRiskRatiosHandler
    : IRequestHandler<GetPortfolioRiskRatiosQuery, Result<GetPortfolioRiskRatiosResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the risk-ratio handler.
    /// FA: Handler نسبت‌های ریسک را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRiskRatiosHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates annualized Sharpe and Sortino ratios from configurable annual arithmetic rates.
    /// FA: نسبت‌های Sharpe و Sortino سالانه‌شده را از نرخ‌های حسابی سالانه قابل‌تنظیم محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Risk-ratio request. FA: درخواست نسبت‌های ریسک.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Portfolio risk ratios. FA: نسبت‌های ریسک پرتفوی.</returns>
    public async Task<Result<GetPortfolioRiskRatiosResponse>> Handle(
        GetPortfolioRiskRatiosQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioRiskStatisticsResponse> statisticsResult =
            await _sender.Send(
                new GetPortfolioRiskStatisticsQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (statisticsResult.IsFailure)
        {
            return Result<GetPortfolioRiskRatiosResponse>.Fail(
                statisticsResult.Error);
        }

        GetPortfolioRiskStatisticsResponse statistics =
            statisticsResult.Value!;

        decimal periodsPerYear =
            statistics.AnnualizationPeriodsPerYear;

        decimal riskFreeRatePeriodic =
            request.RiskFreeRateAnnual /
            periodsPerYear;

        decimal minimumAcceptableReturnPeriodic =
            request.MinimumAcceptableReturnAnnual /
            periodsPerYear;

        decimal annualizationMultiplier =
            (decimal)Math.Sqrt(statistics.AnnualizationPeriodsPerYear);

        decimal? meanExcessOverRiskFree =
            statistics.MeanPeriodicReturn.HasValue
                ? statistics.MeanPeriodicReturn.Value - riskFreeRatePeriodic
                : null;

        decimal? meanExcessOverMinimumAcceptableReturn =
            statistics.MeanPeriodicReturn.HasValue
                ? statistics.MeanPeriodicReturn.Value - minimumAcceptableReturnPeriodic
                : null;

        decimal? downsideDeviationRelativeToMinimumAcceptableReturn =
            CalculateDownsideDeviation(
                statistics.Returns,
                minimumAcceptableReturnPeriodic,
                statistics.IsCalculable);

        bool isSharpeCalculable =
            statistics.IsCalculable &&
            meanExcessOverRiskFree.HasValue &&
            statistics.PeriodicVolatility.HasValue &&
            statistics.PeriodicVolatility.Value > 0m;

        bool isSortinoCalculable =
            statistics.IsCalculable &&
            meanExcessOverMinimumAcceptableReturn.HasValue &&
            downsideDeviationRelativeToMinimumAcceptableReturn.HasValue &&
            downsideDeviationRelativeToMinimumAcceptableReturn.Value > 0m;

        decimal? sharpeRatio =
            isSharpeCalculable
                ? meanExcessOverRiskFree!.Value /
                  statistics.PeriodicVolatility!.Value *
                  annualizationMultiplier
                : null;

        decimal? sortinoRatio =
            isSortinoCalculable
                ? meanExcessOverMinimumAcceptableReturn!.Value /
                  downsideDeviationRelativeToMinimumAcceptableReturn!.Value *
                  annualizationMultiplier
                : null;

        return Result<GetPortfolioRiskRatiosResponse>.Success(
            new GetPortfolioRiskRatiosResponse(
                statistics.PortfolioId,
                statistics.BaseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                statistics.IsComplete,
                statistics.ObservationCount,
                statistics.AnnualizationPeriodsPerYear,
                request.RiskFreeRateAnnual,
                riskFreeRatePeriodic,
                request.MinimumAcceptableReturnAnnual,
                minimumAcceptableReturnPeriodic,
                isSharpeCalculable,
                sharpeRatio,
                isSortinoCalculable,
                sortinoRatio,
                statistics.MeanPeriodicReturn,
                meanExcessOverRiskFree,
                meanExcessOverMinimumAcceptableReturn,
                statistics.PeriodicVolatility,
                downsideDeviationRelativeToMinimumAcceptableReturn));
    }

    private static decimal? CalculateDownsideDeviation(
        IReadOnlyCollection<PortfolioPeriodicReturnResponse> returns,
        decimal targetPeriodic,
        bool isCalculable)
    {
        if (!isCalculable ||
            returns.Count == 0)
        {
            return null;
        }

        decimal downsideSquareSum =
            returns.Sum(
                item =>
                {
                    decimal downside =
                        Math.Min(
                            item.Return - targetPeriodic,
                            0m);

                    return downside * downside;
                });

        decimal downsideVariance =
            downsideSquareSum /
            returns.Count;

        return (decimal)Math.Sqrt((double)downsideVariance);
    }
}
