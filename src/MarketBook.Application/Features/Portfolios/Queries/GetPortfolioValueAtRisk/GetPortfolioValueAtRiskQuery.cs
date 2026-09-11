// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;

/// <summary>
/// EN: Requests historical VaR and CVaR analytics for a portfolio.
/// FA: تحلیل تاریخی VaR و CVaR پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level, e.g. 0.95 or 0.99. FA: سطح اطمینان تاریخی، مانند 0.95 یا 0.99.</param>
public sealed record GetPortfolioValueAtRiskQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    decimal ConfidenceLevel = 0.95m)
    : IRequest<Result<GetPortfolioValueAtRiskResponse>>;
