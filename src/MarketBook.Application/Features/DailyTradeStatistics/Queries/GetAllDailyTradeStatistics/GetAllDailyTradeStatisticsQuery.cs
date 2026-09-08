// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics;

/// <summary>
/// EN: Gets paged daily trade-statistics records.
/// FA: رکوردهای آمار معاملات روزانه را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
/// </summary>
public sealed record GetAllDailyTradeStatisticsQuery(int Page, int PageSize)
    : IRequest<Result<GetAllDailyTradeStatisticsResponse>>;
