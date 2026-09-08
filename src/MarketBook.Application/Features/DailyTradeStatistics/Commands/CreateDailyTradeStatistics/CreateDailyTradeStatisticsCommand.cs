// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics;

/// <summary>
/// EN: Creates daily trade statistics for one Listing and trading date.
/// FA: آمار معاملات روزانه یک Listing و تاریخ معاملاتی را ایجاد می‌کند.
/// </summary>
public sealed record CreateDailyTradeStatisticsCommand(
    string ListingId,
    DateOnly TradingDate,
    long Volume,
    int TradeCount,
    decimal AveragePrice,
    decimal TradeValue,
    decimal MarketCapitalization)
    : IRequest<Result<DailyTradeStatisticsId>>;
