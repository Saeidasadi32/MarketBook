// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav;

/// <summary>
/// EN: FX-translated NAV detail for one source currency.
/// FA: جزئیات NAV ترجمه‌شده با FX برای یک ارز مبدا.
/// </summary>
/// <param name="CurrencyId">EN: Source currency identifier. FA: شناسه ارز مبدا.</param>
/// <param name="SourceCashBalance">EN: Cash balance in source currency. FA: موجودی نقدی به ارز مبدا.</param>
/// <param name="SourcePricedMarketValue">EN: Market value of priced open positions in source currency. FA: ارزش بازار موقعیت‌های باز قیمت‌دار به ارز مبدا.</param>
/// <param name="SourcePricedNetAssetValue">EN: Cash plus priced market value in source currency. FA: وجه نقد به‌علاوه ارزش بازار قیمت‌دار به ارز مبدا.</param>
/// <param name="SourceIsComplete">EN: True when every open position in the source currency is priced. FA: وقتی همه موقعیت‌های باز ارز مبدا قیمت دارند true است.</param>
/// <param name="SourceNetAssetValue">EN: Complete source NAV, or null when any source position is unpriced. FA: NAV کامل ارز مبدا، یا null در صورت وجود موقعیت بدون قیمت.</param>
/// <param name="PricedPositionCount">EN: Number of priced open positions. FA: تعداد موقعیت‌های باز قیمت‌دار.</param>
/// <param name="UnpricedPositionCount">EN: Number of unpriced open positions. FA: تعداد موقعیت‌های باز بدون قیمت.</param>
/// <param name="IsFxAvailable">EN: True when source-to-base translation is available. FA: وقتی نرخ تبدیل ارز مبدا به ارز پایه موجود است true است.</param>
/// <param name="FxRateDate">EN: Effective FX date; null for identity translation. FA: تاریخ موثر نرخ FX؛ برای تبدیل همانی null است.</param>
/// <param name="SourceToBaseRate">EN: Multiplier from source currency to portfolio base currency. FA: ضریب تبدیل ارز مبدا به ارز پایه پرتفوی.</param>
/// <param name="CashBalanceBase">EN: Cash balance translated to base currency. FA: موجودی نقدی ترجمه‌شده به ارز پایه.</param>
/// <param name="PricedMarketValueBase">EN: Priced market value translated to base currency. FA: ارزش بازار قیمت‌دار ترجمه‌شده به ارز پایه.</param>
/// <param name="PricedNetAssetValueBase">EN: Priced NAV translated to base currency. FA: NAV قیمت‌دار ترجمه‌شده به ارز پایه.</param>
/// <param name="NetAssetValueBase">EN: Complete NAV translated to base currency, or null when source NAV is incomplete or FX is missing. FA: NAV کامل ترجمه‌شده به ارز پایه، یا null وقتی NAV مبدا ناقص است یا FX وجود ندارد.</param>
public sealed record PortfolioTranslatedNavCurrencyResponse(
    string CurrencyId,
    decimal SourceCashBalance,
    decimal SourcePricedMarketValue,
    decimal SourcePricedNetAssetValue,
    bool SourceIsComplete,
    decimal? SourceNetAssetValue,
    int PricedPositionCount,
    int UnpricedPositionCount,
    bool IsFxAvailable,
    DateOnly? FxRateDate,
    decimal? SourceToBaseRate,
    decimal? CashBalanceBase,
    decimal? PricedMarketValueBase,
    decimal? PricedNetAssetValueBase,
    decimal? NetAssetValueBase);

/// <summary>
/// EN: Portfolio NAV translated into the configured base currency.
/// FA: NAV پرتفوی ترجمه‌شده به ارز پایه تنظیم‌شده.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Configured portfolio base currency. FA: ارز پایه تنظیم‌شده پرتفوی.</param>
/// <param name="IsPricedNavFullyTranslated">EN: True when every source currency has an FX path to base currency. FA: وقتی برای همه ارزهای مبدا نرخ تبدیل به ارز پایه وجود دارد true است.</param>
/// <param name="IsComplete">EN: True when every source currency has FX and every open position is priced. FA: وقتی همه ارزها FX دارند و همه موقعیت‌های باز قیمت دارند true است.</param>
/// <param name="PricedNetAssetValueBase">EN: Aggregate translated priced NAV, or null when any required FX is missing. FA: مجموع NAV قیمت‌دار ترجمه‌شده، یا null اگر FX لازم برای هر ارز مفقود باشد.</param>
/// <param name="NetAssetValueBase">EN: Aggregate complete translated NAV, or null when pricing or FX is incomplete. FA: مجموع NAV کامل ترجمه‌شده، یا null اگر قیمت‌گذاری یا FX ناقص باشد.</param>
/// <param name="Currencies">EN: Source-currency translation details. FA: جزئیات ترجمه به تفکیک ارز مبدا.</param>
public sealed record GetPortfolioTranslatedNavResponse(
    string PortfolioId,
    string BaseCurrencyId,
    bool IsPricedNavFullyTranslated,
    bool IsComplete,
    decimal? PricedNetAssetValueBase,
    decimal? NetAssetValueBase,
    IReadOnlyCollection<PortfolioTranslatedNavCurrencyResponse> Currencies);
