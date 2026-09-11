// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes;

/// <summary>
/// EN: Derives duration and recovery episodes from the existing drawdown projection.
/// FA: دوره‌های مدت و بازیابی را از Projection موجود افت سرمایه استخراج می‌کند.
/// </summary>
public sealed class GetPortfolioDrawdownEpisodesHandler
    : IRequestHandler<GetPortfolioDrawdownEpisodesQuery, Result<GetPortfolioDrawdownEpisodesResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the drawdown-episode handler.
    /// FA: Handler دوره‌های افت را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioDrawdownEpisodesHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Builds recovered and ongoing drawdown episodes.
    /// FA: دوره‌های افت بازیابی‌شده و جاری را ایجاد می‌کند.
    /// </summary>
    /// <param name="request">EN: Episode request. FA: درخواست دوره‌های افت.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Drawdown duration and recovery analytics. FA: تحلیل مدت افت و بازیابی.</returns>
    public async Task<Result<GetPortfolioDrawdownEpisodesResponse>> Handle(
        GetPortfolioDrawdownEpisodesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioDrawdownResponse> drawdownResult =
            await _sender.Send(
                new GetPortfolioDrawdownQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (drawdownResult.IsFailure)
        {
            return Result<GetPortfolioDrawdownEpisodesResponse>.Fail(
                drawdownResult.Error);
        }

        GetPortfolioDrawdownResponse drawdown = drawdownResult.Value!;

        if (!drawdown.IsComplete)
        {
            return Result<GetPortfolioDrawdownEpisodesResponse>.Success(
                new GetPortfolioDrawdownEpisodesResponse(
                    drawdown.PortfolioId,
                    drawdown.BaseCurrencyId,
                    drawdown.From,
                    drawdown.To,
                    drawdown.Interval,
                    false,
                    false,
                    0,
                    null,
                    null,
                    null,
                    []));
        }

        List<PortfolioDrawdownEpisodeResponse> episodes = [];
        EpisodeBuilder? active = null;

        foreach (PortfolioDrawdownPointResponse point in drawdown.Points)
        {
            if (!point.Drawdown.HasValue ||
                !point.WealthIndex.HasValue ||
                !point.RunningPeakWealthIndex.HasValue ||
                !point.RunningPeakTimestamp.HasValue)
            {
                continue;
            }

            decimal pointDrawdown = point.Drawdown.Value;

            if (active is null)
            {
                if (pointDrawdown < 0m)
                {
                    active = new EpisodeBuilder(
                        point.RunningPeakTimestamp.Value,
                        point.RunningPeakWealthIndex.Value,
                        point.Timestamp,
                        point.WealthIndex.Value,
                        pointDrawdown);
                }

                continue;
            }

            if (pointDrawdown < active.MaximumDrawdown)
            {
                active.TroughTimestamp = point.Timestamp;
                active.TroughWealthIndex = point.WealthIndex.Value;
                active.MaximumDrawdown = pointDrawdown;
            }

            if (pointDrawdown >= 0m)
            {
                episodes.Add(
                    BuildRecoveredEpisode(
                        active,
                        point.Timestamp));

                active = null;
            }
        }

        PortfolioDrawdownEpisodeResponse? activeResponse = null;

        if (active is not null)
        {
            activeResponse =
                BuildOngoingEpisode(
                    active,
                    drawdown.To);

            episodes.Add(activeResponse);
        }

        PortfolioDrawdownEpisodeResponse? maximumDrawdownEpisode =
            episodes
                .OrderBy(episode => episode.MaximumDrawdown)
                .ThenBy(episode => episode.PeakTimestamp)
                .FirstOrDefault();

        PortfolioDrawdownEpisodeResponse? longestDrawdownEpisode =
            episodes
                .OrderByDescending(episode => episode.TotalDurationDays)
                .ThenBy(episode => episode.PeakTimestamp)
                .FirstOrDefault();

        return Result<GetPortfolioDrawdownEpisodesResponse>.Success(
            new GetPortfolioDrawdownEpisodesResponse(
                drawdown.PortfolioId,
                drawdown.BaseCurrencyId,
                drawdown.From,
                drawdown.To,
                drawdown.Interval,
                true,
                activeResponse is not null,
                episodes.Count,
                maximumDrawdownEpisode,
                longestDrawdownEpisode,
                activeResponse,
                episodes));
    }

    private static PortfolioDrawdownEpisodeResponse BuildRecoveredEpisode(
        EpisodeBuilder episode,
        DateTimeOffset recoveryTimestamp)
    {
        decimal peakToTroughDays =
            ToDays(episode.TroughTimestamp - episode.PeakTimestamp);

        decimal recoveryDays =
            ToDays(recoveryTimestamp - episode.TroughTimestamp);

        decimal totalDurationDays =
            ToDays(recoveryTimestamp - episode.PeakTimestamp);

        return new PortfolioDrawdownEpisodeResponse(
            episode.PeakTimestamp,
            episode.PeakWealthIndex,
            episode.TroughTimestamp,
            episode.TroughWealthIndex,
            episode.MaximumDrawdown,
            recoveryTimestamp,
            true,
            peakToTroughDays,
            recoveryDays,
            totalDurationDays);
    }

    private static PortfolioDrawdownEpisodeResponse BuildOngoingEpisode(
        EpisodeBuilder episode,
        DateTimeOffset to)
    {
        return new PortfolioDrawdownEpisodeResponse(
            episode.PeakTimestamp,
            episode.PeakWealthIndex,
            episode.TroughTimestamp,
            episode.TroughWealthIndex,
            episode.MaximumDrawdown,
            null,
            false,
            ToDays(episode.TroughTimestamp - episode.PeakTimestamp),
            null,
            ToDays(to - episode.PeakTimestamp));
    }

    private static decimal ToDays(TimeSpan duration)
        => (decimal)duration.TotalDays;

    private sealed class EpisodeBuilder
    {
        public EpisodeBuilder(
            DateTimeOffset peakTimestamp,
            decimal peakWealthIndex,
            DateTimeOffset troughTimestamp,
            decimal troughWealthIndex,
            decimal maximumDrawdown)
        {
            PeakTimestamp = peakTimestamp;
            PeakWealthIndex = peakWealthIndex;
            TroughTimestamp = troughTimestamp;
            TroughWealthIndex = troughWealthIndex;
            MaximumDrawdown = maximumDrawdown;
        }

        public DateTimeOffset PeakTimestamp { get; }

        public decimal PeakWealthIndex { get; }

        public DateTimeOffset TroughTimestamp { get; set; }

        public decimal TroughWealthIndex { get; set; }

        public decimal MaximumDrawdown { get; set; }
    }
}
