// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;

/// <summary>
/// EN: Requests historical chart-series data for one portfolio.
/// FA: داده سری زمانی تاریخی یک پرتفوی را برای نمودار درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Inclusive beginning instant. FA: لحظه شامل‌شونده شروع.</param>
/// <param name="To">EN: Inclusive ending instant. FA: لحظه شامل‌شونده پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
public sealed record GetPortfolioPerformanceSeriesQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval)
    : IRequest<Result<GetPortfolioPerformanceSeriesResponse>>;
