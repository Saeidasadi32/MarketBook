// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;

/// <summary>
/// EN: Calculates historical-simulation VaR and CVaR from DOC-0033 periodic returns.
/// FA: VaR و CVaR شبیه‌سازی تاریخی را از بازده‌های دوره‌ای DOC-0033 محاسبه می‌کند.
/// </summary>
public sealed class GetPortfolioValueAtRiskHandler
    : IRequestHandler<GetPortfolioValueAtRiskQuery, Result<GetPortfolioValueAtRiskResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the VaR/CVaR handler.
    /// FA: Handler مربوط به VaR/CVaR را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioValueAtRiskHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates historical VaR and CVaR.
    /// FA: VaR و CVaR تاریخی را محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: VaR/CVaR request. FA: درخواست VaR/CVaR.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical VaR/CVaR analytics. FA: تحلیل تاریخی VaR/CVaR.</returns>
    public async Task<Result<GetPortfolioValueAtRiskResponse>> Handle(
        GetPortfolioValueAtRiskQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ConfidenceLevel <= 0m ||
            request.ConfidenceLevel >= 1m)
        {
            return Result<GetPortfolioValueAtRiskResponse>.Fail(
                new Error(
                    "Portfolio.ValueAtRisk.InvalidConfidenceLevel",
                    "ConfidenceLevel must be greater than 0 and less than 1."));
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
            return Result<GetPortfolioValueAtRiskResponse>.Fail(
                statisticsResult.Error);
        }

        GetPortfolioRiskStatisticsResponse statistics =
            statisticsResult.Value!;

        decimal[] returns =
            statistics.Returns
                .Select(item => item.Return)
                .ToArray();

        decimal[] sortedReturns =
            returns
                .OrderBy(item => item)
                .ToArray();

        decimal tailProbability =
            1m - request.ConfidenceLevel;

        if (!statistics.IsComplete)
        {
            return Result<GetPortfolioValueAtRiskResponse>.Success(
                new GetPortfolioValueAtRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    false,
                    false,
                    sortedReturns.Length,
                    request.ConfidenceLevel,
                    tailProbability,
                    null,
                    null,
                    null,
                    0,
                    sortedReturns));
        }

        if (returns.Length < 2)
        {
            return Result<GetPortfolioValueAtRiskResponse>.Success(
                new GetPortfolioValueAtRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    true,
                    false,
                    sortedReturns.Length,
                    request.ConfidenceLevel,
                    tailProbability,
                    null,
                    null,
                    null,
                    0,
                    sortedReturns));
        }

        HistoricalValueAtRiskCalculation calculation =
            HistoricalValueAtRiskCalculator.Calculate(
                returns,
                request.ConfidenceLevel);

        return Result<GetPortfolioValueAtRiskResponse>.Success(
            new GetPortfolioValueAtRiskResponse(
                statistics.PortfolioId,
                statistics.BaseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                true,
                true,
                returns.Length,
                request.ConfidenceLevel,
                tailProbability,
                calculation.ValueAtRiskReturn,
                calculation.ConditionalValueAtRiskReturn,
                calculation.HistoricalQuantileReturn,
                calculation.TailObservationCount,
                calculation.SortedReturns));
    }
}
