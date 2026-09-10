// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;

/// <summary>
/// EN: Requests historical portfolio NAV translated into the configured base currency.
/// FA: NAV تاریخی پرتفوی را پس از ترجمه به ارز پایه تنظیم‌شده درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="AsOf">EN: Inclusive historical cutoff instant. FA: لحظه تاریخی شامل‌شونده.</param>
public sealed record GetPortfolioTranslatedNavAsOfQuery(
    string PortfolioId,
    DateTimeOffset AsOf)
    : IRequest<Result<GetPortfolioTranslatedNavAsOfResponse>>;
