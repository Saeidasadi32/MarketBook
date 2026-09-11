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
/// EN: Composes DOC-0033 risk statistics into Sharpe and Sortino ratios.
/// FA: آمار ریسک DOC-0033 را به نسبت‌های Sharpe و Sortino تبدیل می‌کند.
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
    /// EN: Calculates annualized zero-risk-free Sharpe and zero-target Sortino ratios.
    /// FA: نسبت‌های Sharpe سالانه‌شده با نرخ بدون‌ریسک صفر و Sortino با هدف صفر را محاسبه می‌کند.
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

        decimal annualizationMultiplier =
            (decimal)Math.Sqrt(statistics.AnnualizationPeriodsPerYear);

        bool isSharpeCalculable =
            statistics.IsCalculable &&
            statistics.MeanPeriodicReturn.HasValue &&
            statistics.PeriodicVolatility.HasValue &&
            statistics.PeriodicVolatility.Value > 0m;

        bool isSortinoCalculable =
            statistics.IsCalculable &&
            statistics.MeanPeriodicReturn.HasValue &&
            statistics.PeriodicDownsideDeviation.HasValue &&
            statistics.PeriodicDownsideDeviation.Value > 0m;

        decimal? sharpeRatio =
            isSharpeCalculable
                ? statistics.MeanPeriodicReturn!.Value /
                  statistics.PeriodicVolatility!.Value *
                  annualizationMultiplier
                : null;

        decimal? sortinoRatio =
            isSortinoCalculable
                ? statistics.MeanPeriodicReturn!.Value /
                  statistics.PeriodicDownsideDeviation!.Value *
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
                0m,
                0m,
                isSharpeCalculable,
                sharpeRatio,
                isSortinoCalculable,
                sortinoRatio,
                statistics.MeanPeriodicReturn,
                statistics.PeriodicVolatility,
                statistics.PeriodicDownsideDeviation));
    }
}
