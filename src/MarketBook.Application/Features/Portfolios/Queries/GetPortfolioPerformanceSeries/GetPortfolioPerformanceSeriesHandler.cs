// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;

/// <summary>
/// EN: Builds deterministic daily/weekly chart points by composing historical NAV, TWR, and external-flow projections.
/// FA: نقاط قطعی روزانه/هفتگی نمودار را با ترکیب Projectionهای NAV تاریخی، TWR و جریان‌های خارجی می‌سازد.
/// </summary>
public sealed class GetPortfolioPerformanceSeriesHandler
    : IRequestHandler<GetPortfolioPerformanceSeriesQuery, Result<GetPortfolioPerformanceSeriesResponse>>
{
    private const int MaximumPointCount = 1000;
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the chart-series handler.
    /// FA: Handler سری زمانی نمودار را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to compose existing projections. FA: Sender مدیاتور برای ترکیب Projectionهای موجود.</param>
    public GetPortfolioPerformanceSeriesHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Samples translated NAV and cumulative TWR and returns exact Deposit/Withdrawal markers.
    /// FA: NAV ترجمه‌شده و TWR تجمعی را نمونه‌برداری کرده و markerهای دقیق Deposit/Withdrawal را برمی‌گرداند.
    /// </summary>
    /// <param name="request">EN: Series request. FA: درخواست سری زمانی.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical performance chart series. FA: سری تاریخی نمودار عملکرد.</returns>
    public async Task<Result<GetPortfolioPerformanceSeriesResponse>> Handle(
        GetPortfolioPerformanceSeriesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.From >= request.To)
        {
            return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                new Error(
                    "PortfolioPerformanceSeries.InvalidPeriod",
                    "The series period requires From to be earlier than To."));
        }

        if (!TryResolveInterval(
                request.Interval,
                out string resolvedInterval,
                out int intervalDays))
        {
            return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                new Error(
                    "PortfolioPerformanceSeries.InvalidInterval",
                    "The interval must be Daily or Weekly."));
        }

        List<DateTimeOffset> timestamps =
            BuildTimestamps(
                request.From,
                request.To,
                intervalDays);

        if (timestamps.Count > MaximumPointCount)
        {
            return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                new Error(
                    "PortfolioPerformanceSeries.TooManyPoints",
                    $"The requested series would contain more than {MaximumPointCount} points."));
        }

        Result<GetPortfolioPerformanceResponse> performanceResult =
            await _sender.Send(
                new GetPortfolioPerformanceQuery(
                    request.PortfolioId,
                    request.From,
                    request.To),
                cancellationToken);

        if (performanceResult.IsFailure)
        {
            return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                performanceResult.Error);
        }

        GetPortfolioPerformanceResponse performance = performanceResult.Value!;
        List<PortfolioPerformanceSeriesPointResponse> points = [];
        string? baseCurrencyId = null;

        for (int index = 0; index < timestamps.Count; index++)
        {
            DateTimeOffset timestamp = timestamps[index];

            Result<GetPortfolioTranslatedNavAsOfResponse> navResult =
                await _sender.Send(
                    new GetPortfolioTranslatedNavAsOfQuery(
                        request.PortfolioId,
                        timestamp),
                    cancellationToken);

            if (navResult.IsFailure)
            {
                return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                    navResult.Error);
            }

            GetPortfolioTranslatedNavAsOfResponse nav = navResult.Value!;

            if (baseCurrencyId is null)
            {
                baseCurrencyId = nav.BaseCurrencyId;
            }
            else if (!string.Equals(
                         baseCurrencyId,
                         nav.BaseCurrencyId,
                         StringComparison.Ordinal))
            {
                return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                    new Error(
                        "PortfolioPerformanceSeries.BaseCurrencyMismatch",
                        "Historical NAV points resolved different base currencies."));
            }

            bool isTwrCalculable;
            decimal? cumulativeTwr;

            if (index == 0)
            {
                isTwrCalculable =
                    nav.IsComplete &&
                    nav.NetAssetValueBase.HasValue &&
                    nav.NetAssetValueBase.Value > 0m;

                cumulativeTwr =
                    isTwrCalculable
                        ? 0m
                        : null;
            }
            else
            {
                Result<GetPortfolioTimeWeightedReturnResponse> twrResult =
                    await _sender.Send(
                        new GetPortfolioTimeWeightedReturnQuery(
                            request.PortfolioId,
                            request.From,
                            timestamp),
                        cancellationToken);

                if (twrResult.IsFailure)
                {
                    return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                        twrResult.Error);
                }

                GetPortfolioTimeWeightedReturnResponse twr = twrResult.Value!;

                if (!string.Equals(
                        baseCurrencyId,
                        twr.BaseCurrencyId,
                        StringComparison.Ordinal))
                {
                    return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                        new Error(
                            "PortfolioPerformanceSeries.BaseCurrencyMismatch",
                            "Historical NAV and TWR projections resolved different base currencies."));
                }

                isTwrCalculable =
                    twr.IsCalculable &&
                    twr.TimeWeightedReturn.HasValue;

                cumulativeTwr = twr.TimeWeightedReturn;
            }

            points.Add(
                new PortfolioPerformanceSeriesPointResponse(
                    timestamp,
                    nav.IsComplete,
                    nav.NetAssetValueBase,
                    isTwrCalculable,
                    cumulativeTwr));
        }

        if (!string.Equals(
                baseCurrencyId,
                performance.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioPerformanceSeriesResponse>.Fail(
                new Error(
                    "PortfolioPerformanceSeries.BaseCurrencyMismatch",
                    "Chart points and external-flow projection resolved different base currencies."));
        }

        IReadOnlyCollection<PortfolioPerformanceSeriesFlowMarkerResponse> externalFlows =
            performance.ExternalFlows
                .OrderBy(item => item.OccurredOn)
                .ThenBy(item => item.CashTransactionId, StringComparer.Ordinal)
                .Select(
                    item =>
                        new PortfolioPerformanceSeriesFlowMarkerResponse(
                            item.CashTransactionId,
                            item.CurrencyId,
                            item.Type,
                            item.OccurredOn,
                            item.SignedSourceAmount,
                            item.IsFxAvailable,
                            item.SignedAmountBase))
                .ToArray();

        bool areAllPointsComplete =
            points.All(
                item =>
                    item.IsNavComplete &&
                    item.IsTwrCalculable &&
                    item.NetAssetValueBase.HasValue &&
                    item.CumulativeTimeWeightedReturn.HasValue);

        bool areFlowsTranslated =
            externalFlows.All(item => item.IsFxAvailable);

        return Result<GetPortfolioPerformanceSeriesResponse>.Success(
            new GetPortfolioPerformanceSeriesResponse(
                request.PortfolioId,
                baseCurrencyId!,
                request.From,
                request.To,
                resolvedInterval,
                areAllPointsComplete && areFlowsTranslated,
                points,
                externalFlows));
    }

    private static bool TryResolveInterval(
        string interval,
        out string resolvedInterval,
        out int intervalDays)
    {
        if (string.Equals(
                interval,
                "Daily",
                StringComparison.OrdinalIgnoreCase))
        {
            resolvedInterval = "Daily";
            intervalDays = 1;
            return true;
        }

        if (string.Equals(
                interval,
                "Weekly",
                StringComparison.OrdinalIgnoreCase))
        {
            resolvedInterval = "Weekly";
            intervalDays = 7;
            return true;
        }

        resolvedInterval = string.Empty;
        intervalDays = 0;
        return false;
    }

    private static List<DateTimeOffset> BuildTimestamps(
        DateTimeOffset from,
        DateTimeOffset to,
        int intervalDays)
    {
        List<DateTimeOffset> timestamps = [from];
        DateTimeOffset cursor = from.AddDays(intervalDays);

        while (cursor < to)
        {
            timestamps.Add(cursor);
            cursor = cursor.AddDays(intervalDays);
        }

        if (timestamps[^1] != to)
        {
            timestamps.Add(to);
        }

        return timestamps;
    }
}
