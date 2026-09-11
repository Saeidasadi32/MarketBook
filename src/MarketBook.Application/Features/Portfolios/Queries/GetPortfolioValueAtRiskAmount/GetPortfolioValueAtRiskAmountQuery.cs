// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount;

/// <summary>
/// EN: Requests historical VaR/CVaR ratios and their base-currency amount equivalents.
/// FA: نسبت‌های VaR/CVaR تاریخی و معادل مبلغی آن‌ها در ارز پایه را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant of the return history. FA: لحظه شروع تاریخچه بازده.</param>
/// <param name="To">EN: Ending instant and NAV valuation cutoff. FA: لحظه پایان و نقطه برش ارزش‌گذاری NAV.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
public sealed record GetPortfolioValueAtRiskAmountQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    decimal ConfidenceLevel = 0.95m)
    : IRequest<Result<GetPortfolioValueAtRiskAmountResponse>>;
