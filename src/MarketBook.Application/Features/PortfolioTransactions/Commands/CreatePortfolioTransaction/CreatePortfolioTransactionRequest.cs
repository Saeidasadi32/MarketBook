// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction;

/// <summary>
/// EN: API request for one portfolio transaction.
/// FA: درخواست API برای یک تراکنش پرتفوی.
/// </summary>
public sealed record CreatePortfolioTransactionRequest(
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
    DateTimeOffset ExecutedOn);
