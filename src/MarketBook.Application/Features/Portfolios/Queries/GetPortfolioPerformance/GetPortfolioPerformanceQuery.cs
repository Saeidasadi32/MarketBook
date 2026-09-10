// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;

/// <summary>
/// EN: Requests the cash-flow-aware historical performance foundation for a portfolio.
/// FA: مبنای عملکرد تاریخی پرتفوی را با لحاظ جریان‌های نقدی خارجی درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Inclusive beginning NAV cutoff. FA: لحظه شامل‌شونده برای NAV ابتدای دوره.</param>
/// <param name="To">EN: Inclusive ending NAV cutoff. FA: لحظه شامل‌شونده برای NAV انتهای دوره.</param>
public sealed record GetPortfolioPerformanceQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To)
    : IRequest<Result<GetPortfolioPerformanceResponse>>;
