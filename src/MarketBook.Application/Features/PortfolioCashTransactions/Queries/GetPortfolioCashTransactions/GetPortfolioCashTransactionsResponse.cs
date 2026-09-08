// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions;

/// <summary>EN: One cash transaction in a paged response. FA: یک تراکنش نقدی در پاسخ صفحه‌بندی‌شده.</summary>
public sealed record PortfolioCashTransactionItemResponse(
    string Id,
    string CurrencyId,
    int Type,
    decimal Amount,
    decimal SignedAmount,
    DateTimeOffset OccurredOn,
    string? ReferenceType,
    string? ReferenceId,
    string? Description);

/// <summary>EN: Paged cash-ledger response. FA: پاسخ صفحه‌بندی‌شده دفتر نقدی.</summary>
public sealed record GetPortfolioCashTransactionsResponse(
    IReadOnlyList<PortfolioCashTransactionItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
