// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison;

/// <summary>
/// EN: Requests a side-by-side TWR and XIRR comparison for a historical portfolio period.
/// FA: مقایسه کنارهم TWR و XIRR را برای یک دوره تاریخی پرتفوی درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning valuation instant. FA: لحظه ارزش‌گذاری ابتدای دوره.</param>
/// <param name="To">EN: Ending valuation instant. FA: لحظه ارزش‌گذاری انتهای دوره.</param>
public sealed record GetPortfolioPerformanceComparisonQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To)
    : IRequest<Result<GetPortfolioPerformanceComparisonResponse>>;
