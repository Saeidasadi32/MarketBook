// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById;

/// <summary>EN: Detailed cash transaction response. FA: پاسخ جزئیات تراکنش نقدی.</summary>
public sealed record GetPortfolioCashTransactionByIdResponse(
    string Id,
    string PortfolioId,
    string CurrencyId,
    int Type,
    decimal Amount,
    decimal SignedAmount,
    DateTimeOffset OccurredOn,
    string? ReferenceType,
    string? ReferenceId,
    string? Description,
    DateTimeOffset CreatedOn);
