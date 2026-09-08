// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById;

/// <summary>
/// EN: Gets daily trade statistics by identifier.
/// FA: آمار معاملات روزانه را با شناسه دریافت می‌کند.
/// </summary>
public sealed record GetDailyTradeStatisticsByIdQuery(string Id)
    : IRequest<Result<GetDailyTradeStatisticsByIdResponse>>;
