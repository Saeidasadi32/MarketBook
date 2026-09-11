// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk;

/// <summary>
/// EN: Derives rolling historical VaR/CVaR from DOC-0033 periodic returns using the DOC-0037 calculator.
/// FA: VaR/CVaR تاریخی Rolling را از بازده‌های دوره‌ای DOC-0033 و با محاسبه‌گر DOC-0037 استخراج می‌کند.
/// </summary>
public sealed class GetPortfolioRollingValueAtRiskHandler
    : IRequestHandler<GetPortfolioRollingValueAtRiskQuery, Result<GetPortfolioRollingValueAtRiskResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the rolling VaR/CVaR handler.
    /// FA: Handler مربوط به VaR/CVaR Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRollingValueAtRiskHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates full-window rolling historical VaR/CVaR.
    /// FA: VaR/CVaR تاریخی Rolling را فقط برای پنجره‌های کامل محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Rolling VaR/CVaR request. FA: درخواست VaR/CVaR Rolling.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Rolling historical VaR/CVaR analytics. FA: تحلیل Rolling مربوط به VaR/CVaR تاریخی.</returns>
    public async Task<Result<GetPortfolioRollingValueAtRiskResponse>> Handle(
        GetPortfolioRollingValueAtRiskQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.WindowPeriods < 2)
        {
            return Result<GetPortfolioRollingValueAtRiskResponse>.Fail(
                new Error(
                    "Portfolio.RollingValueAtRisk.InvalidWindowPeriods",
                    "WindowPeriods must be at least 2."));
        }

        if (request.WindowPeriods > 1000)
        {
            return Result<GetPortfolioRollingValueAtRiskResponse>.Fail(
                new Error(
                    "Portfolio.RollingValueAtRisk.WindowTooLarge",
                    "WindowPeriods cannot exceed 1000."));
        }

        if (request.ConfidenceLevel <= 0m ||
            request.ConfidenceLevel >= 1m)
        {
            return Result<GetPortfolioRollingValueAtRiskResponse>.Fail(
                new Error(
                    "Portfolio.RollingValueAtRisk.InvalidConfidenceLevel",
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
            return Result<GetPortfolioRollingValueAtRiskResponse>.Fail(
                statisticsResult.Error);
        }

        GetPortfolioRiskStatisticsResponse statistics =
            statisticsResult.Value!;

        decimal tailProbability =
            1m - request.ConfidenceLevel;

        if (!statistics.IsComplete)
        {
            return Result<GetPortfolioRollingValueAtRiskResponse>.Success(
                new GetPortfolioRollingValueAtRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    false,
                    request.WindowPeriods,
                    request.ConfidenceLevel,
                    tailProbability,
                    0,
                    []));
        }

        PortfolioPeriodicReturnResponse[] returns =
            statistics.Returns.ToArray();

        if (returns.Length < request.WindowPeriods)
        {
            return Result<GetPortfolioRollingValueAtRiskResponse>.Success(
                new GetPortfolioRollingValueAtRiskResponse(
                    statistics.PortfolioId,
                    statistics.BaseCurrencyId,
                    statistics.From,
                    statistics.To,
                    statistics.Interval,
                    true,
                    request.WindowPeriods,
                    request.ConfidenceLevel,
                    tailProbability,
                    0,
                    []));
        }

        List<PortfolioRollingValueAtRiskPointResponse> points = [];

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

            HistoricalValueAtRiskCalculation calculation =
                HistoricalValueAtRiskCalculator.Calculate(
                    window.Select(item => item.Return),
                    request.ConfidenceLevel);

            points.Add(
                new PortfolioRollingValueAtRiskPointResponse(
                    window[0].From,
                    window[^1].To,
                    window.Length,
                    calculation.HistoricalQuantileReturn,
                    calculation.ValueAtRiskReturn,
                    calculation.ConditionalValueAtRiskReturn,
                    calculation.TailObservationCount));
        }

        return Result<GetPortfolioRollingValueAtRiskResponse>.Success(
            new GetPortfolioRollingValueAtRiskResponse(
                statistics.PortfolioId,
                statistics.BaseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                true,
                request.WindowPeriods,
                request.ConfidenceLevel,
                tailProbability,
                points.Count,
                points));
    }
}
