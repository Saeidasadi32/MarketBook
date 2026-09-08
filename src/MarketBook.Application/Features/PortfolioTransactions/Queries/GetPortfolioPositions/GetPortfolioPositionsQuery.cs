// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions;

/// <summary>
/// EN: Queries the current open-position projection for one portfolio.
/// FA: Projection موقعیت‌های باز فعلی یک پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">
/// EN: Portfolio identifier.
/// FA: شناسه پرتفوی.
/// </param>
public sealed record GetPortfolioPositionsQuery(string PortfolioId)
    : IRequest<Result<GetPortfolioPositionsResponse>>;
