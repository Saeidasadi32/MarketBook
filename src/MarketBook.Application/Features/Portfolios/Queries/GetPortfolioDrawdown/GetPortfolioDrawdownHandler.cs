// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown;

/// <summary>
/// EN: Calculates peak-to-trough drawdown from the cash-flow-neutral cumulative TWR wealth index.
/// FA: افت قله تا کف را از شاخص ثروت TWR تجمعی و خنثی نسبت به جریان سرمایه محاسبه می‌کند.
/// </summary>
public sealed class GetPortfolioDrawdownHandler
    : IRequestHandler<GetPortfolioDrawdownQuery, Result<GetPortfolioDrawdownResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the drawdown handler.
    /// FA: Handler افت سرمایه را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioDrawdownHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates running peaks, current drawdown, and maximum drawdown from cumulative TWR.
    /// FA: قله جاری، افت فعلی و بیشینه افت را از TWR تجمعی محاسبه می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioDrawdownResponse>> Handle(
        GetPortfolioDrawdownQuery request,
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
            return Result<GetPortfolioDrawdownResponse>.Fail(seriesResult.Error);
        }

        GetPortfolioPerformanceSeriesResponse series = seriesResult.Value!;
        List<PortfolioDrawdownPointResponse> points = [];

        decimal? runningPeak = null;
        DateTimeOffset? runningPeakTimestamp = null;
        decimal? maximumDrawdown = null;
        DateTimeOffset? maximumDrawdownPeakTimestamp = null;
        DateTimeOffset? maximumDrawdownTroughTimestamp = null;
        bool isComplete = true;

        foreach (PortfolioPerformanceSeriesPointResponse sourcePoint in series.Points)
        {
            if (!sourcePoint.IsTwrCalculable ||
                !sourcePoint.CumulativeTimeWeightedReturn.HasValue)
            {
                isComplete = false;

                points.Add(
                    new PortfolioDrawdownPointResponse(
                        sourcePoint.Timestamp,
                        false,
                        sourcePoint.CumulativeTimeWeightedReturn,
                        null,
                        runningPeak,
                        runningPeakTimestamp,
                        null));

                continue;
            }

            decimal wealthIndex =
                1m + sourcePoint.CumulativeTimeWeightedReturn.Value;

            if (wealthIndex <= 0m)
            {
                isComplete = false;

                points.Add(
                    new PortfolioDrawdownPointResponse(
                        sourcePoint.Timestamp,
                        false,
                        sourcePoint.CumulativeTimeWeightedReturn,
                        wealthIndex,
                        runningPeak,
                        runningPeakTimestamp,
                        null));

                continue;
            }

            if (!runningPeak.HasValue || wealthIndex > runningPeak.Value)
            {
                runningPeak = wealthIndex;
                runningPeakTimestamp = sourcePoint.Timestamp;
            }

            decimal drawdown =
                wealthIndex / runningPeak.Value - 1m;

            if (!maximumDrawdown.HasValue || drawdown < maximumDrawdown.Value)
            {
                maximumDrawdown = drawdown;
                maximumDrawdownPeakTimestamp = runningPeakTimestamp;
                maximumDrawdownTroughTimestamp = sourcePoint.Timestamp;
            }

            points.Add(
                new PortfolioDrawdownPointResponse(
                    sourcePoint.Timestamp,
                    true,
                    sourcePoint.CumulativeTimeWeightedReturn,
                    wealthIndex,
                    runningPeak,
                    runningPeakTimestamp,
                    drawdown));
        }

        decimal? currentDrawdown =
            points.Count > 0
                ? points[^1].Drawdown
                : null;

        return Result<GetPortfolioDrawdownResponse>.Success(
            new GetPortfolioDrawdownResponse(
                series.PortfolioId,
                series.BaseCurrencyId,
                series.From,
                series.To,
                series.Interval,
                isComplete,
                runningPeak,
                runningPeakTimestamp,
                currentDrawdown,
                maximumDrawdown,
                maximumDrawdownPeakTimestamp,
                maximumDrawdownTroughTimestamp,
                points));
    }
}
