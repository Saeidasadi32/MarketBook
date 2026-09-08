// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency;

/// <summary>
/// EN: Sets the reporting/base currency of a portfolio.
/// FA: ارز پایه/گزارش‌دهی یک پرتفوی را تنظیم می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="CurrencyId">EN: Currency identifier. FA: شناسه ارز.</param>
public sealed record SetPortfolioBaseCurrencyCommand(
    string PortfolioId,
    string CurrencyId)
    : IRequest<Result<PortfolioId>>;
