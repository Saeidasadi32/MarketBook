// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction;

/// <summary>
/// EN: Creates one immutable portfolio ledger transaction.
/// FA: یک تراکنش تغییرناپذیر در دفتر پرتفوی ایجاد می‌کند.
/// </summary>
public sealed record CreatePortfolioTransactionCommand(
    string PortfolioId,
    string ListingId,
    int Type,
    decimal Quantity,
    decimal Price,
    decimal Commission,
    decimal Tax,
    decimal ExchangeFee,
    decimal BrokerFee,
    decimal ClearingFee,
    decimal OtherFees,
    DateTimeOffset ExecutedOn)
    : IRequest<Result<PortfolioTransactionId>>;
