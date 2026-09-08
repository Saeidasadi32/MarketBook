// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency;

/// <summary>
/// EN: HTTP request for setting a portfolio base currency.
/// FA: درخواست HTTP برای تنظیم ارز پایه پرتفوی.
/// </summary>
/// <param name="CurrencyId">EN: Currency identifier. FA: شناسه ارز.</param>
public sealed record SetPortfolioBaseCurrencyRequest(string CurrencyId);
