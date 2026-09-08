// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction;

/// <summary>
/// EN: Command to append one immutable cash-ledger transaction.
/// FA: فرمان افزودن یک تراکنش تغییرناپذیر به دفتر نقدی.
/// </summary>
public sealed record CreatePortfolioCashTransactionCommand(
    string PortfolioId,
    string CurrencyId,
    int Type,
    decimal Amount,
    DateTimeOffset OccurredOn,
    string? ReferenceType,
    string? ReferenceId,
    string? Description)
    : IRequest<Result<PortfolioCashTransactionId>>;
