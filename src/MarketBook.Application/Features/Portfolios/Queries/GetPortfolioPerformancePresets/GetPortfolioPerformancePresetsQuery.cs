// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets;

/// <summary>
/// EN: Requests standard dashboard performance periods ending at one deterministic as-of instant.
/// FA: دوره‌های استاندارد عملکرد داشبورد را تا یک لحظه قطعی as-of درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="AsOf">EN: Common ending instant for every preset. FA: لحظه پایان مشترک برای همه بازه‌ها.</param>
public sealed record GetPortfolioPerformancePresetsQuery(
    string PortfolioId,
    DateTimeOffset AsOf)
    : IRequest<Result<GetPortfolioPerformancePresetsResponse>>;
