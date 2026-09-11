// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes;

/// <summary>
/// EN: Requests drawdown-duration and recovery analytics.
/// FA: تحلیل مدت افت و بازیابی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
public sealed record GetPortfolioDrawdownEpisodesQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval)
    : IRequest<Result<GetPortfolioDrawdownEpisodesResponse>>;
