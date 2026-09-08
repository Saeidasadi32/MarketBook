// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics;

/// <summary>
/// EN: Represents the request used to create daily trade statistics.
/// FA: درخواست ایجاد آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed record CreateDailyTradeStatisticsRequest(
    string ListingId,
    DateOnly TradingDate,
    long Volume,
    int TradeCount,
    decimal AveragePrice,
    decimal TradeValue,
    decimal MarketCapitalization);
