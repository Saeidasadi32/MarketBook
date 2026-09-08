// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio;

/// <summary>
/// EN: Requests creation of a portfolio.
/// FA: درخواست ایجاد یک پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record CreatePortfolioCommand(
    string InvestorId,
    string Name) : IRequest<Result<PortfolioId>>;
