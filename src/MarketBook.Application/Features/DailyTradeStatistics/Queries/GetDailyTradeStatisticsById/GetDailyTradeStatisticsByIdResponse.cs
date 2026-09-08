// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById;

/// <summary>
/// EN: Represents one daily trade-statistics record.
/// FA: یک رکورد آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed record GetDailyTradeStatisticsByIdResponse(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    long Volume,
    int TradeCount,
    decimal AveragePrice,
    decimal TradeValue,
    decimal MarketCapitalization,
    decimal AverageTradeSize,
    DateTimeOffset CreatedOn,
    DateTimeOffset UpdatedOn);
