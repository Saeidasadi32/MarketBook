// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio;

/// <summary>
/// EN: Requests portfolio deactivate.
/// FA: درخواست غیرفعال‌سازی پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record DeactivatePortfolioCommand(
    string Id) : IRequest<Result<PortfolioId>>;
