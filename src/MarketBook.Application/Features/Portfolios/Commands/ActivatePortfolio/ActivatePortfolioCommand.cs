// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio;

/// <summary>
/// EN: Requests portfolio activate.
/// FA: درخواست فعال‌سازی پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record ActivatePortfolioCommand(
    string Id) : IRequest<Result<PortfolioId>>;
