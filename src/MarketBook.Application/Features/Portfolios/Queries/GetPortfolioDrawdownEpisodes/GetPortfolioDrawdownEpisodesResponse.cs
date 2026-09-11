// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes;

/// <summary>
/// EN: One continuous drawdown episode from a peak until recovery or the requested To boundary.
/// FA: یک دوره پیوسته افت از قله تا بازیابی یا مرز To در درخواست.
/// </summary>
/// <param name="PeakTimestamp">EN: Peak that starts the episode. FA: قله آغازکننده دوره.</param>
/// <param name="PeakWealthIndex">EN: TWR wealth index at the peak. FA: شاخص ثروت TWR در قله.</param>
/// <param name="TroughTimestamp">EN: Deepest observed point in the episode. FA: زمان عمیق‌ترین نقطه مشاهده‌شده در دوره.</param>
/// <param name="TroughWealthIndex">EN: Wealth index at the trough. FA: شاخص ثروت در کف.</param>
/// <param name="MaximumDrawdown">EN: Most negative drawdown in the episode. FA: منفی‌ترین افت در دوره.</param>
/// <param name="RecoveryTimestamp">EN: First sampled instant back at or above the episode peak; null when ongoing. FA: اولین لحظه نمونه‌برداری در سطح قله یا بالاتر؛ برای دوره جاری null است.</param>
/// <param name="IsRecovered">EN: True when the episode recovered inside the requested range. FA: وقتی دوره در بازه درخواستی بازیابی شده باشد true است.</param>
/// <param name="PeakToTroughDays">EN: Elapsed days from peak to trough. FA: روزهای سپری‌شده از قله تا کف.</param>
/// <param name="RecoveryDays">EN: Elapsed days from trough to recovery; null when ongoing. FA: روزهای سپری‌شده از کف تا بازیابی؛ برای دوره جاری null است.</param>
/// <param name="TotalDurationDays">EN: Peak-to-recovery days, or peak-to-To days when ongoing. FA: روزهای قله تا بازیابی، یا قله تا To برای دوره جاری.</param>
public sealed record PortfolioDrawdownEpisodeResponse(
    DateTimeOffset PeakTimestamp,
    decimal PeakWealthIndex,
    DateTimeOffset TroughTimestamp,
    decimal TroughWealthIndex,
    decimal MaximumDrawdown,
    DateTimeOffset? RecoveryTimestamp,
    bool IsRecovered,
    decimal PeakToTroughDays,
    decimal? RecoveryDays,
    decimal TotalDurationDays);

/// <summary>
/// EN: Portfolio drawdown duration and recovery analytics.
/// FA: تحلیل مدت افت و بازیابی پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: False when drawdown continuity cannot be proven because source data is incomplete. FA: وقتی به دلیل داده ناقص پیوستگی افت قابل اثبات نباشد false است.</param>
/// <param name="HasActiveDrawdown">EN: True when To is still below the active peak. FA: وقتی در To هنوز زیر قله فعال باشیم true است.</param>
/// <param name="EpisodeCount">EN: Number of detected episodes. FA: تعداد دوره‌های افت شناسایی‌شده.</param>
/// <param name="MaximumDrawdownEpisode">EN: Episode with the deepest drawdown. FA: دوره دارای عمیق‌ترین افت.</param>
/// <param name="LongestDrawdownEpisode">EN: Episode with the longest total duration. FA: دوره دارای طولانی‌ترین مدت کل.</param>
/// <param name="ActiveDrawdownEpisode">EN: Ongoing episode at To, if any. FA: دوره جاری در To، در صورت وجود.</param>
/// <param name="Episodes">EN: Ordered drawdown episodes. FA: دوره‌های افت به ترتیب زمانی.</param>
public sealed record GetPortfolioDrawdownEpisodesResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    bool HasActiveDrawdown,
    int EpisodeCount,
    PortfolioDrawdownEpisodeResponse? MaximumDrawdownEpisode,
    PortfolioDrawdownEpisodeResponse? LongestDrawdownEpisode,
    PortfolioDrawdownEpisodeResponse? ActiveDrawdownEpisode,
    IReadOnlyCollection<PortfolioDrawdownEpisodeResponse> Episodes);
