// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio;

/// <summary>
/// EN: HTTP request model for portfolio creation.
/// FA: مدل درخواست HTTP برای ایجاد پرتفوی.
/// </summary>
public sealed record CreatePortfolioRequest(
    string InvestorId,
    string Name);
