// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl;

/// <summary>
/// EN: Requests portfolio total P/L translated into the portfolio base currency.
/// FA: سود/زیان کل پرتفوی را پس از ترجمه به ارز پایه پرتفوی درخواست می‌کند.
/// </summary>
public sealed record GetPortfolioTranslatedTotalPnlQuery(
    string PortfolioId,
    string? ListingId)
    : IRequest<Result<GetPortfolioTranslatedTotalPnlResponse>>;
