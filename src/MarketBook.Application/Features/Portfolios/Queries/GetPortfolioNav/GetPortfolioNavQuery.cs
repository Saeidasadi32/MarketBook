// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;

/// <summary>
/// EN: Requests the current portfolio NAV grouped by currency.
/// FA: NAV جاری پرتفوی را به تفکیک ارز درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
public sealed record GetPortfolioNavQuery(string PortfolioId)
    : IRequest<Result<GetPortfolioNavResponse>>;
