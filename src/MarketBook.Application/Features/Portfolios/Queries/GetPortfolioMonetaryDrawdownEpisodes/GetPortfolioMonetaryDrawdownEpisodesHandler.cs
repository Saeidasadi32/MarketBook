// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdownEpisodes
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdownEpisodes;

/// <summary>
/// EN: Composes DOC-0032 episode semantics with DOC-0041 monetary drawdown points.
/// FA: semantics دوره‌های DOC-0032 را با نقاط افت مبلغی DOC-0041 ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioMonetaryDrawdownEpisodesHandler
    : IRequestHandler<GetPortfolioMonetaryDrawdownEpisodesQuery, Result<GetPortfolioMonetaryDrawdownEpisodesResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the monetary drawdown episode handler.
    /// FA: Handler دوره‌های افت مبلغی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioMonetaryDrawdownEpisodesHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Enriches each drawdown episode with trough, recovery, and active monetary amounts.
    /// FA: هر دوره افت را با مبالغ کف، بازیابی و افت فعال غنی می‌کند.
    /// </summary>
    /// <param name="request">EN: Monetary episode request. FA: درخواست دوره افت مبلغی.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Monetary drawdown episode analytics. FA: تحلیل دوره‌های افت مبلغی.</returns>
    public async Task<Result<GetPortfolioMonetaryDrawdownEpisodesResponse>> Handle(
        GetPortfolioMonetaryDrawdownEpisodesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioDrawdownEpisodesResponse> episodeResult =
            await _sender.Send(
                new GetPortfolioDrawdownEpisodesQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (episodeResult.IsFailure)
        {
            return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Fail(
                episodeResult.Error);
        }

        GetPortfolioDrawdownEpisodesResponse episodeSource =
            episodeResult.Value!;

        if (!episodeSource.IsComplete)
        {
            return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Success(
                CreateIncompleteResponse(episodeSource));
        }

        Result<GetPortfolioMonetaryDrawdownResponse> monetaryResult =
            await _sender.Send(
                new GetPortfolioMonetaryDrawdownQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (monetaryResult.IsFailure)
        {
            return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Fail(
                monetaryResult.Error);
        }

        GetPortfolioMonetaryDrawdownResponse monetarySource =
            monetaryResult.Value!;

        if (!string.Equals(
                episodeSource.BaseCurrencyId,
                monetarySource.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Fail(
                new Error(
                    "Portfolio.MonetaryDrawdownEpisodes.BaseCurrencyMismatch",
                    "Drawdown episodes and monetary drawdown resolved different base currencies."));
        }

        if (!monetarySource.IsComplete)
        {
            return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Success(
                CreateIncompleteResponse(episodeSource));
        }

        Dictionary<DateTimeOffset, PortfolioMonetaryDrawdownPointResponse> monetaryByTimestamp =
            monetarySource.Points.ToDictionary(
                point => point.Timestamp);

        List<PortfolioMonetaryDrawdownEpisodeResponse> episodes = [];
        bool allComplete = true;

        foreach (PortfolioDrawdownEpisodeResponse episode in episodeSource.Episodes)
        {
            PortfolioMonetaryDrawdownEpisodeResponse mapped =
                MapEpisode(
                    episode,
                    request.To,
                    monetaryByTimestamp);

            allComplete &=
                mapped.IsComplete;

            episodes.Add(mapped);
        }

        PortfolioMonetaryDrawdownEpisodeResponse? maximumDrawdownEpisode =
            MapSummaryEpisode(
                episodeSource.MaximumDrawdownEpisode,
                episodes);

        PortfolioMonetaryDrawdownEpisodeResponse? longestDrawdownEpisode =
            MapSummaryEpisode(
                episodeSource.LongestDrawdownEpisode,
                episodes);

        PortfolioMonetaryDrawdownEpisodeResponse? activeDrawdownEpisode =
            MapSummaryEpisode(
                episodeSource.ActiveDrawdownEpisode,
                episodes);

        return Result<GetPortfolioMonetaryDrawdownEpisodesResponse>.Success(
            new GetPortfolioMonetaryDrawdownEpisodesResponse(
                episodeSource.PortfolioId,
                episodeSource.BaseCurrencyId,
                episodeSource.From,
                episodeSource.To,
                episodeSource.Interval,
                allComplete,
                episodeSource.HasActiveDrawdown,
                episodeSource.EpisodeCount,
                maximumDrawdownEpisode,
                longestDrawdownEpisode,
                activeDrawdownEpisode,
                episodes));
    }

    private static GetPortfolioMonetaryDrawdownEpisodesResponse CreateIncompleteResponse(
        GetPortfolioDrawdownEpisodesResponse source)
        => new(
            source.PortfolioId,
            source.BaseCurrencyId,
            source.From,
            source.To,
            source.Interval,
            false,
            false,
            0,
            null,
            null,
            null,
            []);

    private static PortfolioMonetaryDrawdownEpisodeResponse MapEpisode(
        PortfolioDrawdownEpisodeResponse episode,
        DateTimeOffset to,
        IReadOnlyDictionary<DateTimeOffset, PortfolioMonetaryDrawdownPointResponse> monetaryByTimestamp)
    {
        bool hasTrough =
            monetaryByTimestamp.TryGetValue(
                episode.TroughTimestamp,
                out PortfolioMonetaryDrawdownPointResponse? troughPoint);

        bool troughComplete =
            hasTrough &&
            troughPoint is not null &&
            troughPoint.IsComplete &&
            troughPoint.IsCalculable &&
            troughPoint.DrawdownAmountBase.HasValue;

        decimal? recoveryNetAssetValueBase = null;
        bool recoveryComplete = true;

        if (episode.IsRecovered &&
            episode.RecoveryTimestamp.HasValue)
        {
            bool hasRecovery =
                monetaryByTimestamp.TryGetValue(
                    episode.RecoveryTimestamp.Value,
                    out PortfolioMonetaryDrawdownPointResponse? recoveryPoint);

            recoveryComplete =
                hasRecovery &&
                recoveryPoint is not null &&
                recoveryPoint.IsComplete &&
                recoveryPoint.IsCalculable;

            if (recoveryComplete)
            {
                recoveryNetAssetValueBase =
                    recoveryPoint!.NetAssetValueBase;
            }
        }

        decimal? currentDrawdownAmountBase = null;
        bool activeComplete = true;

        if (!episode.IsRecovered)
        {
            bool hasCurrent =
                monetaryByTimestamp.TryGetValue(
                    to,
                    out PortfolioMonetaryDrawdownPointResponse? currentPoint);

            activeComplete =
                hasCurrent &&
                currentPoint is not null &&
                currentPoint.IsComplete &&
                currentPoint.IsCalculable &&
                currentPoint.DrawdownAmountBase.HasValue;

            if (activeComplete)
            {
                currentDrawdownAmountBase =
                    currentPoint!.DrawdownAmountBase;
            }
        }

        bool isComplete =
            troughComplete &&
            recoveryComplete &&
            activeComplete;

        decimal? recoveredDrawdownAmountBase =
            episode.IsRecovered &&
            troughComplete
                ? troughPoint!.DrawdownAmountBase
                : null;

        return new PortfolioMonetaryDrawdownEpisodeResponse(
            episode.PeakTimestamp,
            episode.PeakWealthIndex,
            episode.TroughTimestamp,
            episode.TroughWealthIndex,
            episode.MaximumDrawdown,
            troughComplete
                ? troughPoint!.NetAssetValueBase
                : null,
            troughComplete
                ? troughPoint!.EquivalentPeakNetAssetValueBase
                : null,
            troughComplete
                ? troughPoint!.DrawdownAmountBase
                : null,
            episode.RecoveryTimestamp,
            recoveryNetAssetValueBase,
            recoveredDrawdownAmountBase,
            currentDrawdownAmountBase,
            episode.IsRecovered,
            episode.PeakToTroughDays,
            episode.RecoveryDays,
            episode.TotalDurationDays,
            isComplete);
    }

    private static PortfolioMonetaryDrawdownEpisodeResponse? MapSummaryEpisode(
        PortfolioDrawdownEpisodeResponse? source,
        IReadOnlyCollection<PortfolioMonetaryDrawdownEpisodeResponse> mapped)
    {
        if (source is null)
        {
            return null;
        }

        return mapped.FirstOrDefault(
            item =>
                item.PeakTimestamp == source.PeakTimestamp &&
                item.TroughTimestamp == source.TroughTimestamp);
    }
}
