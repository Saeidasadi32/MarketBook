// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk;

/// <summary>
/// EN: Requests rolling historical VaR and CVaR analytics.
/// FA: تحلیل Rolling مربوط به VaR و CVaR تاریخی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="WindowPeriods">EN: Number of periodic returns per full rolling window. FA: تعداد بازده‌های دوره‌ای در هر پنجره کامل Rolling.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
public sealed record GetPortfolioRollingValueAtRiskQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    int WindowPeriods = 30,
    decimal ConfidenceLevel = 0.95m)
    : IRequest<Result<GetPortfolioRollingValueAtRiskResponse>>;
