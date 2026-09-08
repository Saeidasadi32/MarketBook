// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById;

/// <summary>
/// EN: Queries one portfolio transaction by identifier.
/// FA: یک تراکنش پرتفوی را با شناسه درخواست می‌کند.
/// </summary>
/// <param name="Id">EN: Transaction identifier. FA: شناسه تراکنش.</param>
public sealed record GetPortfolioTransactionByIdQuery(string Id)
    : IRequest<Result<GetPortfolioTransactionByIdResponse>>;
