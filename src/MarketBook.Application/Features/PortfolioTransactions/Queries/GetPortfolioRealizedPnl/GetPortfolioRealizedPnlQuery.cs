// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl;

/// <summary>
/// EN: Requests realized profit/loss reconstructed from the immutable trade ledger.
/// FA: سود/زیان تحقق‌یافته بازسازی‌شده از دفتر تغییرناپذیر معاملات را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="ListingId">EN: Optional listing filter. FA: فیلتر اختیاری لیستینگ.</param>
/// <param name="CostBasisMethod">EN: Cost-basis method code. FA: کد روش بهای تمام‌شده.</param>
public sealed record GetPortfolioRealizedPnlQuery(
    string PortfolioId,
    string? ListingId,
    int CostBasisMethod)
    : IRequest<Result<GetPortfolioRealizedPnlResponse>>;
