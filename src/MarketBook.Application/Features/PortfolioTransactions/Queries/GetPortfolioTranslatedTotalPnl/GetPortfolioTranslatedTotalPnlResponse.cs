// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl;

/// <summary>
/// EN: FX translation detail for one source currency.
/// FA: جزئیات ترجمه FX برای یک ارز مبدا.
/// </summary>
public sealed record PortfolioTranslatedPnlCurrencyResponse(
    string CurrencyId,
    bool IsFullyPriced,
    decimal SourceRealizedProfitLoss,
    decimal? SourceUnrealizedProfitLoss,
    decimal? SourceTotalProfitLoss,
    bool IsFxAvailable,
    DateOnly? FxRateDate,
    decimal? SourceToBaseRate,
    decimal? RealizedProfitLossBase,
    decimal? UnrealizedProfitLossBase,
    decimal? TotalProfitLossBase);

/// <summary>
/// EN: Portfolio P/L translated into its configured base currency.
/// FA: سود/زیان پرتفوی ترجمه‌شده به ارز پایه تنظیم‌شده آن.
/// </summary>
public sealed record GetPortfolioTranslatedTotalPnlResponse(
    string PortfolioId,
    string BaseCurrencyId,
    bool IsRealizedFullyTranslated,
    bool IsFullyPricedAndTranslated,
    decimal? RealizedProfitLossBase,
    decimal? UnrealizedProfitLossBase,
    decimal? TotalProfitLossBase,
    IReadOnlyCollection<PortfolioTranslatedPnlCurrencyResponse> Currencies);
