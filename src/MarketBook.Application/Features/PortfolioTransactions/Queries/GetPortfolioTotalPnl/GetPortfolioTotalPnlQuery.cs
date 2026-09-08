// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl;

/// <summary>
/// EN: Requests realized, unrealized, and total portfolio P/L without cross-currency aggregation.
/// FA: سود/زیان تحقق‌یافته، تحقق‌نیافته و کل پرتفوی را بدون تجمیع بین ارزها درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="ListingId">EN: Optional listing filter. FA: فیلتر اختیاری لیستینگ.</param>
public sealed record GetPortfolioTotalPnlQuery(
    string PortfolioId,
    string? ListingId)
    : IRequest<Result<GetPortfolioTotalPnlResponse>>;
