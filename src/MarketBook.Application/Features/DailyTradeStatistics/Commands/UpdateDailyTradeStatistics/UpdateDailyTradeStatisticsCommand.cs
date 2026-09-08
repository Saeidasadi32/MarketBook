// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics;

/// <summary>
/// EN: Updates daily trade statistics.
/// FA: آمار معاملات روزانه را به‌روزرسانی می‌کند.
/// </summary>
public sealed record UpdateDailyTradeStatisticsCommand(
    string Id,
    long Volume,
    int TradeCount,
    decimal AveragePrice,
    decimal TradeValue,
    decimal MarketCapitalization)
    : IRequest<Result<DailyTradeStatisticsId>>;
