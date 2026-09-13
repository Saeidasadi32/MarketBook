// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdownEpisodes;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary;

/// <summary>
/// EN: Composes existing portfolio risk projections into a compact dashboard.
/// FA: projectionهای موجود ریسک پرتفوی را در یک داشبورد فشرده ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioRiskSummaryHandler
    : IRequestHandler<GetPortfolioRiskSummaryQuery, Result<GetPortfolioRiskSummaryResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the portfolio risk-summary handler.
    /// FA: Handler خلاصه ریسک پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRiskSummaryHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Builds the risk dashboard exclusively by composing existing analytics.
    /// FA: داشبورد ریسک را صرفاً با ترکیب تحلیل‌های موجود می‌سازد.
    /// </summary>
    /// <param name="request">EN: Risk-summary request. FA: درخواست خلاصه ریسک.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Compact portfolio risk dashboard. FA: داشبورد فشرده ریسک پرتفوی.</returns>
    public async Task<Result<GetPortfolioRiskSummaryResponse>> Handle(
        GetPortfolioRiskSummaryQuery request,
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
            return Result<GetPortfolioRiskSummaryResponse>.Fail(statisticsResult.Error);
        }

        Result<GetPortfolioRiskRatiosResponse> ratiosResult =
            await _sender.Send(
                new GetPortfolioRiskRatiosQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual),
                cancellationToken);

        if (ratiosResult.IsFailure)
        {
            return Result<GetPortfolioRiskSummaryResponse>.Fail(ratiosResult.Error);
        }

        Result<GetPortfolioValueAtRiskAmountResponse> valueAtRiskResult =
            await _sender.Send(
                new GetPortfolioValueAtRiskAmountQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.ConfidenceLevel),
                cancellationToken);

        if (valueAtRiskResult.IsFailure)
        {
            return Result<GetPortfolioRiskSummaryResponse>.Fail(valueAtRiskResult.Error);
        }

        Result<GetPortfolioMonetaryDrawdownResponse> drawdownResult =
            await _sender.Send(
                new GetPortfolioMonetaryDrawdownQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (drawdownResult.IsFailure)
        {
            return Result<GetPortfolioRiskSummaryResponse>.Fail(drawdownResult.Error);
        }

        Result<GetPortfolioMonetaryDrawdownEpisodesResponse> episodesResult =
            await _sender.Send(
                new GetPortfolioMonetaryDrawdownEpisodesQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (episodesResult.IsFailure)
        {
            return Result<GetPortfolioRiskSummaryResponse>.Fail(episodesResult.Error);
        }

        GetPortfolioRiskStatisticsResponse statistics = statisticsResult.Value!;
        GetPortfolioRiskRatiosResponse ratios = ratiosResult.Value!;
        GetPortfolioValueAtRiskAmountResponse valueAtRisk = valueAtRiskResult.Value!;
        GetPortfolioMonetaryDrawdownResponse drawdown = drawdownResult.Value!;
        GetPortfolioMonetaryDrawdownEpisodesResponse episodes = episodesResult.Value!;

        string baseCurrencyId = statistics.BaseCurrencyId;

        if (!HasSameBaseCurrency(
                baseCurrencyId,
                ratios.BaseCurrencyId,
                valueAtRisk.BaseCurrencyId,
                drawdown.BaseCurrencyId,
                episodes.BaseCurrencyId))
        {
            return Result<GetPortfolioRiskSummaryResponse>.Fail(
                new Error(
                    "Portfolio.RiskSummary.BaseCurrencyMismatch",
                    "Composed risk projections resolved different base currencies."));
        }

        PortfolioRiskStatisticsSummaryResponse statisticsSummary =
            new(
                statistics.IsComplete,
                statistics.IsCalculable,
                statistics.ObservationCount,
                statistics.AnnualizationPeriodsPerYear,
                statistics.MeanPeriodicReturn,
                statistics.AnnualizedVolatility,
                statistics.AnnualizedDownsideDeviation);

        PortfolioRiskRatiosSummaryResponse ratiosSummary =
            new(
                ratios.IsComplete,
                ratios.RiskFreeRateAnnual,
                ratios.MinimumAcceptableReturnAnnual,
                ratios.IsSharpeCalculable,
                ratios.SharpeRatio,
                ratios.IsSortinoCalculable,
                ratios.SortinoRatio);

        PortfolioValueAtRiskSummaryResponse valueAtRiskSummary =
            new(
                valueAtRisk.IsComplete,
                valueAtRisk.IsCalculable,
                valueAtRisk.ConfidenceLevel,
                valueAtRisk.ObservationCount,
                valueAtRisk.TailObservationCount,
                valueAtRisk.NetAssetValueBase,
                valueAtRisk.ValueAtRiskReturn,
                valueAtRisk.ConditionalValueAtRiskReturn,
                valueAtRisk.ValueAtRiskAmountBase,
                valueAtRisk.ConditionalValueAtRiskAmountBase);

        PortfolioDrawdownSummaryResponse drawdownSummary =
            new(
                drawdown.IsComplete,
                drawdown.CurrentDrawdown,
                drawdown.CurrentDrawdownAmountBase,
                drawdown.MaximumDrawdown,
                drawdown.MaximumDrawdownAmountBase,
                drawdown.MaximumDrawdownPeakTimestamp,
                drawdown.MaximumDrawdownTroughTimestamp);

        PortfolioDrawdownEpisodesSummaryResponse episodesSummary =
            new(
                episodes.IsComplete,
                episodes.EpisodeCount,
                episodes.HasActiveDrawdown,
                episodes.MaximumDrawdownEpisode?.TroughDrawdownAmountBase,
                episodes.MaximumDrawdownEpisode?.TotalDurationDays,
                episodes.LongestDrawdownEpisode?.TotalDurationDays,
                episodes.ActiveDrawdownEpisode?.CurrentDrawdownAmountBase);

        bool isComplete =
            statistics.IsComplete &&
            ratios.IsComplete &&
            valueAtRisk.IsComplete &&
            drawdown.IsComplete &&
            episodes.IsComplete;

        return Result<GetPortfolioRiskSummaryResponse>.Success(
            new GetPortfolioRiskSummaryResponse(
                statistics.PortfolioId,
                baseCurrencyId,
                statistics.From,
                statistics.To,
                statistics.Interval,
                isComplete,
                statisticsSummary,
                ratiosSummary,
                valueAtRiskSummary,
                drawdownSummary,
                episodesSummary));
    }

    private static bool HasSameBaseCurrency(
        string expected,
        params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (!string.Equals(expected, candidate, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
