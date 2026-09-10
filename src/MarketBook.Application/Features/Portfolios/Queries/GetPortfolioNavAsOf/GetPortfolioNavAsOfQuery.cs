// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;

/// <summary>
/// EN: Requests portfolio NAV reconstructed at a historical cutoff instant.
/// FA: NAV پرتفوی را در یک لحظه تاریخی مشخص بازسازی می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="AsOf">EN: Inclusive historical cutoff instant. FA: لحظه تاریخی شامل‌شونده برای برش داده‌ها.</param>
public sealed record GetPortfolioNavAsOfQuery(
    string PortfolioId,
    DateTimeOffset AsOf)
    : IRequest<Result<GetPortfolioNavAsOfResponse>>;
