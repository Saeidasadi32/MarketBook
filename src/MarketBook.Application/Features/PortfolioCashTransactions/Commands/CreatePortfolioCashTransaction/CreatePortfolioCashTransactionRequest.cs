// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction;

/// <summary>
/// EN: HTTP request model for appending a portfolio cash transaction.
/// FA: مدل درخواست HTTP برای افزودن یک تراکنش دفتر نقدی پرتفوی.
/// </summary>
public sealed record CreatePortfolioCashTransactionRequest(
    string PortfolioId,
    string CurrencyId,
    int Type,
    decimal Amount,
    DateTimeOffset OccurredOn,
    string? ReferenceType,
    string? ReferenceId,
    string? Description);
