// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;

/// <summary>
/// EN: Derives periodic TWR returns, volatility, and downside deviation from the existing performance series.
/// FA: بازده‌های دوره‌ای TWR، نوسان و انحراف نزولی را از سری عملکرد موجود استخراج می‌کند.
/// </summary>
public sealed class GetPortfolioRiskStatisticsHandler
    : IRequestHandler<GetPortfolioRiskStatisticsQuery, Result<GetPortfolioRiskStatisticsResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the risk-statistics handler.
    /// FA: Handler آمار ریسک را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRiskStatisticsHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates sample volatility and zero-target downside deviation.
    /// FA: نوسان نمونه و انحراف نزولی با هدف صفر را محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Risk-statistics request. FA: درخواست آمار ریسک.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Portfolio risk statistics. FA: آمار ریسک پرتفوی.</returns>
    public async Task<Result<GetPortfolioRiskStatisticsResponse>> Handle(
        GetPortfolioRiskStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioPerformanceSeriesResponse> seriesResult =
            await _sender.Send(
                new GetPortfolioPerformanceSeriesQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (seriesResult.IsFailure)
        {
            return Result<GetPortfolioRiskStatisticsResponse>.Fail(
                seriesResult.Error);
        }

        GetPortfolioPerformanceSeriesResponse series = seriesResult.Value!;
        int annualizationPeriodsPerYear =
            string.Equals(
                series.Interval,
                "Daily",
                StringComparison.Ordinal)
                ? 365
                : 52;

        PortfolioPerformanceSeriesPointResponse[] points =
            series.Points.ToArray();

        List<PortfolioPeriodicReturnResponse> returns = [];

        bool sourceIsComplete =
            series.IsComplete &&
            points.All(
                point =>
                    point.IsTwrCalculable &&
                    point.CumulativeTimeWeightedReturn.HasValue);

        if (sourceIsComplete)
        {
            for (int index = 1; index < points.Length; index++)
            {
                decimal priorWealth =
                    1m + points[index - 1].CumulativeTimeWeightedReturn!.Value;

                decimal currentWealth =
                    1m + points[index].CumulativeTimeWeightedReturn!.Value;

                if (priorWealth <= 0m ||
                    currentWealth <= 0m)
                {
                    sourceIsComplete = false;
                    returns.Clear();
                    break;
                }

                decimal periodReturn =
                    currentWealth / priorWealth - 1m;

                returns.Add(
                    new PortfolioPeriodicReturnResponse(
                        points[index - 1].Timestamp,
                        points[index].Timestamp,
                        periodReturn));
            }
        }

        bool isCalculable =
            sourceIsComplete &&
            returns.Count >= 2;

        if (!isCalculable)
        {
            return Result<GetPortfolioRiskStatisticsResponse>.Success(
                new GetPortfolioRiskStatisticsResponse(
                    series.PortfolioId,
                    series.BaseCurrencyId,
                    series.From,
                    series.To,
                    series.Interval,
                    sourceIsComplete,
                    false,
                    returns.Count,
                    annualizationPeriodsPerYear,
                    null,
                    null,
                    null,
                    null,
                    null,
                    returns));
        }

        decimal mean =
            returns.Average(item => item.Return);

        decimal sumSquaredDeviations =
            returns.Sum(
                item =>
                {
                    decimal difference = item.Return - mean;
                    return difference * difference;
                });

        decimal sampleVariance =
            sumSquaredDeviations /
            (returns.Count - 1);

        decimal periodicVolatility =
            SquareRoot(sampleVariance);

        decimal downsideSquareSum =
            returns.Sum(
                item =>
                {
                    decimal downside =
                        Math.Min(item.Return, 0m);

                    return downside * downside;
                });

        decimal downsideVariance =
            downsideSquareSum /
            returns.Count;

        decimal periodicDownsideDeviation =
            SquareRoot(downsideVariance);

        decimal annualizationMultiplier =
            SquareRoot(annualizationPeriodsPerYear);

        return Result<GetPortfolioRiskStatisticsResponse>.Success(
            new GetPortfolioRiskStatisticsResponse(
                series.PortfolioId,
                series.BaseCurrencyId,
                series.From,
                series.To,
                series.Interval,
                true,
                true,
                returns.Count,
                annualizationPeriodsPerYear,
                mean,
                periodicVolatility,
                periodicVolatility * annualizationMultiplier,
                periodicDownsideDeviation,
                periodicDownsideDeviation * annualizationMultiplier,
                returns));
    }

    private static decimal SquareRoot(decimal value)
        => (decimal)Math.Sqrt((double)value);

    private static decimal SquareRoot(int value)
        => (decimal)Math.Sqrt(value);
}
