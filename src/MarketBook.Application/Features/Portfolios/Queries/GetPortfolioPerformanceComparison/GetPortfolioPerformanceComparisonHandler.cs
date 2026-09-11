// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison;

/// <summary>
/// EN: Composes the existing TWR and XIRR projections without reimplementing either return methodology.
/// FA: Projectionهای موجود TWR و XIRR را بدون پیاده‌سازی دوباره هیچ‌یک از روش‌های بازده ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioPerformanceComparisonHandler
    : IRequestHandler<GetPortfolioPerformanceComparisonQuery, Result<GetPortfolioPerformanceComparisonResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the performance comparison handler.
    /// FA: Handler مقایسه عملکرد را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to compose existing return projections. FA: Sender مدیاتور برای ترکیب Projectionهای موجود بازده.</param>
    public GetPortfolioPerformanceComparisonHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Executes TWR and XIRR projections for the same period and returns a unified comparison.
    /// FA: Projectionهای TWR و XIRR را برای یک دوره یکسان اجرا کرده و مقایسه‌ای یکپارچه برمی‌گرداند.
    /// </summary>
    /// <param name="request">EN: Comparison request. FA: درخواست مقایسه.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Combined TWR/XIRR comparison. FA: مقایسه ترکیبی TWR/XIRR.</returns>
    public async Task<Result<GetPortfolioPerformanceComparisonResponse>> Handle(
        GetPortfolioPerformanceComparisonQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.From >= request.To)
        {
            return Result<GetPortfolioPerformanceComparisonResponse>.Fail(
                new Error(
                    "PortfolioPerformanceComparison.InvalidPeriod",
                    "The comparison period requires From to be earlier than To."));
        }

        Result<GetPortfolioTimeWeightedReturnResponse> twrResult =
            await _sender.Send(
                new GetPortfolioTimeWeightedReturnQuery(
                    request.PortfolioId,
                    request.From,
                    request.To),
                cancellationToken);

        if (twrResult.IsFailure)
        {
            return Result<GetPortfolioPerformanceComparisonResponse>.Fail(
                twrResult.Error);
        }

        Result<GetPortfolioMoneyWeightedReturnResponse> xirrResult =
            await _sender.Send(
                new GetPortfolioMoneyWeightedReturnQuery(
                    request.PortfolioId,
                    request.From,
                    request.To),
                cancellationToken);

        if (xirrResult.IsFailure)
        {
            return Result<GetPortfolioPerformanceComparisonResponse>.Fail(
                xirrResult.Error);
        }

        GetPortfolioTimeWeightedReturnResponse twr = twrResult.Value!;
        GetPortfolioMoneyWeightedReturnResponse xirr = xirrResult.Value!;

        if (!string.Equals(
                twr.BaseCurrencyId,
                xirr.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioPerformanceComparisonResponse>.Fail(
                new Error(
                    "PortfolioPerformanceComparison.BaseCurrencyMismatch",
                    "The composed TWR and XIRR projections resolved different base currencies."));
        }

        bool isDataComplete =
            twr.IsComplete &&
            xirr.IsComplete;

        bool areBothReturnsAvailable =
            twr.IsCalculable &&
            twr.TimeWeightedReturn.HasValue &&
            xirr.HasSolution &&
            xirr.AnnualizedMoneyWeightedReturn.HasValue;

        decimal? spread =
            areBothReturnsAvailable
                ? xirr.AnnualizedMoneyWeightedReturn!.Value -
                  twr.TimeWeightedReturn!.Value
                : null;

        return Result<GetPortfolioPerformanceComparisonResponse>.Success(
            new GetPortfolioPerformanceComparisonResponse(
                request.PortfolioId,
                twr.BaseCurrencyId,
                request.From,
                request.To,
                isDataComplete,
                areBothReturnsAvailable,
                twr.IsComplete,
                twr.IsCalculable,
                twr.TimeWeightedReturn,
                twr.ExternalFlowBoundaryCount,
                xirr.IsComplete,
                xirr.HasValidCashFlowSigns,
                xirr.HasSolution,
                xirr.AnnualizedMoneyWeightedReturn,
                xirr.CashFlows.Count,
                spread));
    }
}
