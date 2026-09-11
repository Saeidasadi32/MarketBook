// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;

/// <summary>
/// EN: Requests historical time-weighted return for a portfolio.
/// FA: بازده زمانی‌وزن تاریخی یک پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Inclusive beginning valuation instant. FA: لحظه شامل‌شونده ارزش‌گذاری ابتدای دوره.</param>
/// <param name="To">EN: Inclusive ending valuation instant. FA: لحظه شامل‌شونده ارزش‌گذاری انتهای دوره.</param>
public sealed record GetPortfolioTimeWeightedReturnQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To)
    : IRequest<Result<GetPortfolioTimeWeightedReturnResponse>>;
