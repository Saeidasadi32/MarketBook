// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav;

/// <summary>
/// EN: Requests current portfolio NAV translated into the configured base currency.
/// FA: NAV جاری پرتفوی را پس از ترجمه به ارز پایه تنظیم‌شده درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
public sealed record GetPortfolioTranslatedNavQuery(string PortfolioId)
    : IRequest<Result<GetPortfolioTranslatedNavResponse>>;
