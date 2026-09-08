// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions;

/// <summary>
/// EN: Queries a paged transaction ledger for one portfolio.
/// FA: دفتر تراکنش صفحه‌بندی‌شده یک پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="Page">EN: Requested page. FA: صفحه درخواستی.</param>
/// <param name="PageSize">EN: Requested page size. FA: اندازه صفحه درخواستی.</param>
public sealed record GetPortfolioTransactionsQuery(
    string PortfolioId,
    int Page,
    int PageSize)
    : IRequest<Result<GetPortfolioTransactionsResponse>>;
