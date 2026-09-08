// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances;

/// <summary>EN: Query to project cash balances from the immutable ledger. FA: Query محاسبه موجودی‌های نقدی از دفتر تغییرناپذیر.</summary>
public sealed record GetPortfolioCashBalancesQuery(string PortfolioId)
    : IRequest<Result<GetPortfolioCashBalancesResponse>>;
