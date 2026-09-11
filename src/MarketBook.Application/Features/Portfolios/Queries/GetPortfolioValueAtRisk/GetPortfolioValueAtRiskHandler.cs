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
    public GetPortfolioValueAtRiskHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates historical VaR and CVaR.
    /// FA: VaR و CVaR تاریخی را محاسبه می‌کند.
    /// </summary>
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

        decimal[] sortedReturns =
            statistics.Returns
                .Select(item => item.Return)
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

        if (sortedReturns.Length < 2)
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

        int quantileRank =
            (int)Math.Ceiling(
                (double)(tailProbability * sortedReturns.Length));

        quantileRank =
            Math.Clamp(
                quantileRank,
                1,
                sortedReturns.Length);

        int quantileIndex =
            quantileRank - 1;

        decimal quantileReturn =
            sortedReturns[quantileIndex];

        decimal valueAtRiskReturn =
            Math.Max(
                -quantileReturn,
                0m);

        decimal[] tailReturns =
            sortedReturns
                .Where(item => item <= quantileReturn)
                .ToArray();

        decimal averageTailReturn =
            tailReturns.Average();

        decimal conditionalValueAtRiskReturn =
            Math.Max(
                -averageTailReturn,
                0m);

        return Result<GetPortfolioValueAtRiskResponse>.Success(
            new GetPortfolioValueAtRiskResponse(
                statistics.PortfolioId,
                statistics.BaseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                true,
                true,
                sortedReturns.Length,
                request.ConfidenceLevel,
                tailProbability,
                valueAtRiskReturn,
                conditionalValueAtRiskReturn,
                quantileReturn,
                tailReturns.Length,
                sortedReturns));
    }
}
