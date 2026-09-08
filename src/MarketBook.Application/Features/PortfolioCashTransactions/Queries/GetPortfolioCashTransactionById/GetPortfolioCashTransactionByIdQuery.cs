// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById;

/// <summary>EN: Query for one cash-ledger transaction. FA: Query دریافت یک تراکنش دفتر نقدی.</summary>
public sealed record GetPortfolioCashTransactionByIdQuery(string Id)
    : IRequest<Result<GetPortfolioCashTransactionByIdResponse>>;
