// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation;

/// <summary>
/// EN: Requests current portfolio valuation and unrealized P/L from open positions.
/// FA: ارزش‌گذاری جاری پرتفوی و سود/زیان تحقق‌نیافته موقعیت‌های باز را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="ListingId">EN: Optional listing filter. FA: فیلتر اختیاری لیستینگ.</param>
public sealed record GetPortfolioValuationQuery(
    string PortfolioId,
    string? ListingId)
    : IRequest<Result<GetPortfolioValuationResponse>>;
