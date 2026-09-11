// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk;

/// <summary>
/// EN: Derives rolling risk analytics from DOC-0033 periodic returns.
/// FA: تحلیل ریسک Rolling را از بازده‌های دوره‌ای DOC-0033 استخراج می‌کند.
/// </summary>
public sealed class GetPortfolioRollingRiskHandler
    : IRequestHandler<GetPortfolioRollingRiskQuery, Result<GetPortfolioRollingRiskResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the rolling-risk handler.
    /// FA: Handler ریسک Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRollingRiskHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates rolling volatility, downside deviation, Sharpe, and Sortino.
    /// FA: نوسان، انحراف نزولی، Sharpe و Sortino Rolling را محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Rolling-risk request. FA: درخواست ریسک Rolling.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Rolling risk analytics. FA: تحلیل ریسک Rolling.</returns>
    public async Task<Result<GetPortfolioRollingRiskResponse>> Handle(
        GetPortfolioRollingRiskQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.WindowPeriods < 2)
        {
            return Result<GetPortfolioRollingRiskResponse>.Fail(
                new Error(
                    "Portfolio.RollingRisk.InvalidWindowPeriods",
                    "WindowPeriods must be at least 2."));
        }

        if (request.WindowPeriods > 1000)
        {
            return Result<GetPortfolioRollingRiskResponse>.Fail(
                new Error(
                    "Portfolio.RollingRisk.WindowTooLarge",
                    "WindowPeriods cannot exceed 1000."));
        }

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
            return Result<GetPortfolioRollingRiskResponse>.Fail(
                statisticsResult.Error);
        }

        GetPortfolioRiskStatisticsResponse statistics =
            statisticsResult.Value!;

        if (!statistics.IsComplete)
        {
            return Result<GetPortfolioRollingRiskResponse>.Success(
                new GetPortfolioRollingRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    false,
                    request.WindowPeriods,
                    statistics.AnnualizationPeriodsPerYear,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual,
                    0,
                    []));
        }

        PortfolioPeriodicReturnResponse[] returns =
            statistics.Returns.ToArray();

        if (returns.Length < request.WindowPeriods)
        {
            return Result<GetPortfolioRollingRiskResponse>.Success(
                new GetPortfolioRollingRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    true,
                    request.WindowPeriods,
                    statistics.AnnualizationPeriodsPerYear,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual,
                    0,
                    []));
        }

        decimal periodsPerYear =
            statistics.AnnualizationPeriodsPerYear;

        decimal riskFreePeriodic =
            request.RiskFreeRateAnnual /
            periodsPerYear;

        decimal marPeriodic =
            request.MinimumAcceptableReturnAnnual /
            periodsPerYear;

        decimal annualizationMultiplier =
            (decimal)Math.Sqrt(statistics.AnnualizationPeriodsPerYear);

        List<PortfolioRollingRiskPointResponse> points = [];

        for (int endIndex = request.WindowPeriods - 1;
             endIndex < returns.Length;
             endIndex++)
        {
            int startIndex =
                endIndex - request.WindowPeriods + 1;

            PortfolioPeriodicReturnResponse[] window =
                returns
                    .Skip(startIndex)
                    .Take(request.WindowPeriods)
                    .ToArray();

            decimal mean =
                window.Average(item => item.Return);

            decimal sumSquaredDeviations =
                window.Sum(
                    item =>
                    {
                        decimal difference =
                            item.Return - mean;

                        return difference * difference;
                    });

            decimal sampleVariance =
                sumSquaredDeviations /
                (window.Length - 1);

            decimal periodicVolatility =
                SquareRoot(sampleVariance);

            decimal downsideSquareSum =
                window.Sum(
                    item =>
                    {
                        decimal downside =
                            Math.Min(
                                item.Return - marPeriodic,
                                0m);

                        return downside * downside;
                    });

            decimal downsideVariance =
                downsideSquareSum /
                window.Length;

            decimal periodicDownsideDeviation =
                SquareRoot(downsideVariance);

            decimal meanExcessRiskFree =
                mean - riskFreePeriodic;

            decimal meanExcessMar =
                mean - marPeriodic;

            decimal? sharpe =
                periodicVolatility > 0m
                    ? meanExcessRiskFree /
                      periodicVolatility *
                      annualizationMultiplier
                    : null;

            decimal? sortino =
                periodicDownsideDeviation > 0m
                    ? meanExcessMar /
                      periodicDownsideDeviation *
                      annualizationMultiplier
                    : null;

            points.Add(
                new PortfolioRollingRiskPointResponse(
                    window[0].From,
                    window[^1].To,
                    window.Length,
                    mean,
                    periodicVolatility,
                    periodicVolatility * annualizationMultiplier,
                    periodicDownsideDeviation,
                    periodicDownsideDeviation * annualizationMultiplier,
                    sharpe,
                    sortino));
        }

        return Result<GetPortfolioRollingRiskResponse>.Success(
            new GetPortfolioRollingRiskResponse(
                statistics.PortfolioId,
                statistics.BaseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                true,
                request.WindowPeriods,
                statistics.AnnualizationPeriodsPerYear,
                request.RiskFreeRateAnnual,
                request.MinimumAcceptableReturnAnnual,
                points.Count,
                points));
    }

    private static decimal SquareRoot(decimal value)
        => (decimal)Math.Sqrt((double)value);
}
