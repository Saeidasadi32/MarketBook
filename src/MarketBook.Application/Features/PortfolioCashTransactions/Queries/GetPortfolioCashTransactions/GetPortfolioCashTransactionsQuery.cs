// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions;

/// <summary>EN: Query for paged portfolio cash transactions. FA: Query دریافت صفحه‌بندی‌شده تراکنش‌های نقدی پرتفوی.</summary>
public sealed record GetPortfolioCashTransactionsQuery(
    string PortfolioId,
    string? CurrencyId,
    int Page,
    int PageSize)
    : IRequest<Result<GetPortfolioCashTransactionsResponse>>;
