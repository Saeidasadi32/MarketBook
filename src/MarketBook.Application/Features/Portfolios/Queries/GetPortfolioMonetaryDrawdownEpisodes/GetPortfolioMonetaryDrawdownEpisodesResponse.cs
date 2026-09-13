// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdownEpisodes
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdownEpisodes;

/// <summary>
/// EN: One drawdown episode enriched with base-currency monetary measurements.
/// FA: یک دوره افت همراه با سنجه‌های مبلغی در ارز پایه.
/// </summary>
/// <param name="PeakTimestamp">EN: Episode peak timestamp. FA: زمان قله دوره.</param>
/// <param name="PeakWealthIndex">EN: TWR wealth index at peak. FA: شاخص ثروت TWR در قله.</param>
/// <param name="TroughTimestamp">EN: Deepest observed timestamp. FA: زمان عمیق‌ترین کف مشاهده‌شده.</param>
/// <param name="TroughWealthIndex">EN: Wealth index at trough. FA: شاخص ثروت در کف.</param>
/// <param name="MaximumDrawdown">EN: Most negative relative drawdown in the episode. FA: منفی‌ترین افت نسبی دوره.</param>
/// <param name="TroughNetAssetValueBase">EN: Historical base-currency NAV at trough. FA: NAV تاریخی ارز پایه در کف.</param>
/// <param name="TroughEquivalentPeakNetAssetValueBase">EN: Peak-equivalent NAV at trough capital scale. FA: NAV معادل قله با مقیاس سرمایه کف.</param>
/// <param name="TroughDrawdownAmountBase">EN: Monetary loss represented by the trough drawdown. FA: زیان مبلغی متناظر با افت کف.</param>
/// <param name="RecoveryTimestamp">EN: Recovery timestamp; null when ongoing. FA: زمان بازیابی؛ برای دوره جاری null است.</param>
/// <param name="RecoveryNetAssetValueBase">EN: Historical NAV at recovery; null when ongoing or unavailable. FA: NAV تاریخی در بازیابی؛ برای دوره جاری یا داده ناموجود null است.</param>
/// <param name="RecoveredDrawdownAmountBase">EN: Amount recovered from trough back to peak-equivalent level; null when ongoing. FA: مبلغ جبران‌شده از کف تا سطح معادل قله؛ برای دوره جاری null است.</param>
/// <param name="CurrentDrawdownAmountBase">EN: Monetary drawdown at To for the active episode; null for recovered episodes. FA: مبلغ افت در To برای دوره فعال؛ برای دوره بازیابی‌شده null است.</param>
/// <param name="IsRecovered">EN: True when recovered within the requested range. FA: وقتی دوره در بازه بازیابی شده باشد true است.</param>
/// <param name="PeakToTroughDays">EN: Elapsed days from peak to trough. FA: روزهای قله تا کف.</param>
/// <param name="RecoveryDays">EN: Elapsed days from trough to recovery. FA: روزهای کف تا بازیابی.</param>
/// <param name="TotalDurationDays">EN: Peak-to-recovery or peak-to-To duration. FA: مدت قله تا بازیابی یا قله تا To.</param>
/// <param name="IsComplete">EN: True when all monetary measurements required by the episode are complete. FA: وقتی همه سنجه‌های مبلغی لازم دوره کامل باشند true است.</param>
public sealed record PortfolioMonetaryDrawdownEpisodeResponse(
    DateTimeOffset PeakTimestamp,
    decimal PeakWealthIndex,
    DateTimeOffset TroughTimestamp,
    decimal TroughWealthIndex,
    decimal MaximumDrawdown,
    decimal? TroughNetAssetValueBase,
    decimal? TroughEquivalentPeakNetAssetValueBase,
    decimal? TroughDrawdownAmountBase,
    DateTimeOffset? RecoveryTimestamp,
    decimal? RecoveryNetAssetValueBase,
    decimal? RecoveredDrawdownAmountBase,
    decimal? CurrentDrawdownAmountBase,
    bool IsRecovered,
    decimal PeakToTroughDays,
    decimal? RecoveryDays,
    decimal TotalDurationDays,
    bool IsComplete);

/// <summary>
/// EN: Drawdown episodes with base-currency monetary loss and recovery analytics.
/// FA: دوره‌های افت همراه با تحلیل زیان و بازیابی مبلغی در ارز پایه.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when source episodes and all monetary mappings are complete. FA: وقتی دوره‌های مبنا و همه نگاشت‌های مبلغی کامل باشند true است.</param>
/// <param name="HasActiveDrawdown">EN: True when an episode is still active at To. FA: وقتی در To یک دوره افت فعال باشد true است.</param>
/// <param name="EpisodeCount">EN: Episode count. FA: تعداد دوره‌ها.</param>
/// <param name="MaximumDrawdownEpisode">EN: Deepest relative-drawdown episode. FA: دوره دارای عمیق‌ترین افت نسبی.</param>
/// <param name="LongestDrawdownEpisode">EN: Longest-duration episode. FA: طولانی‌ترین دوره افت.</param>
/// <param name="ActiveDrawdownEpisode">EN: Active episode at To. FA: دوره فعال در To.</param>
/// <param name="Episodes">EN: Ordered enriched episodes. FA: دوره‌های غنی‌شده به ترتیب زمانی.</param>
public sealed record GetPortfolioMonetaryDrawdownEpisodesResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    bool HasActiveDrawdown,
    int EpisodeCount,
    PortfolioMonetaryDrawdownEpisodeResponse? MaximumDrawdownEpisode,
    PortfolioMonetaryDrawdownEpisodeResponse? LongestDrawdownEpisode,
    PortfolioMonetaryDrawdownEpisodeResponse? ActiveDrawdownEpisode,
    IReadOnlyCollection<PortfolioMonetaryDrawdownEpisodeResponse> Episodes);
