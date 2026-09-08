// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio;

/// <summary>
/// EN: Requests a portfolio rename.
/// FA: درخواست تغییر نام پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record UpdatePortfolioCommand(
    string Id,
    string Name) : IRequest<Result<PortfolioId>>;

/// <summary>
/// EN: HTTP request model for portfolio update.
/// FA: مدل درخواست HTTP برای به‌روزرسانی پرتفوی.
/// </summary>
public sealed record UpdatePortfolioRequest(string Name);
