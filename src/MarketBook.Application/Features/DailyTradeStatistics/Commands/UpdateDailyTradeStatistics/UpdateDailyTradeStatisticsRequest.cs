// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics;

/// <summary>
/// EN: Represents writable daily trade-statistics fields.
/// FA: فیلدهای قابل تغییر آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed record UpdateDailyTradeStatisticsRequest(
    long Volume,
    int TradeCount,
    decimal AveragePrice,
    decimal TradeValue,
    decimal MarketCapitalization);
