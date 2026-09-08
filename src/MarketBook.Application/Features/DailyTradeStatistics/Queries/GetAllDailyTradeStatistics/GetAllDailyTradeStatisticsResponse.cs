// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics;

/// <summary>
/// EN: Represents paged daily trade-statistics records.
/// FA: پاسخ صفحه‌بندی‌شده آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed record GetAllDailyTradeStatisticsResponse(
    IReadOnlyCollection<DailyTradeStatisticsListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents one daily trade-statistics list item.
/// FA: یک آیتم فهرست آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed record DailyTradeStatisticsListItemResponse(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    long Volume,
    int TradeCount,
    decimal TradeValue,
    decimal MarketCapitalization);
