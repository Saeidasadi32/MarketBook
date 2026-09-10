// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;

/// <summary>
/// EN: Historical NAV translation detail for one source currency.
/// FA: جزئیات ترجمه NAV تاریخی برای یک ارز مبدا.
/// </summary>
/// <param name="CurrencyId">EN: Source currency identifier. FA: شناسه ارز مبدا.</param>
/// <param name="SourceCashBalance">EN: Historical source cash balance. FA: موجودی نقدی تاریخی در ارز مبدا.</param>
/// <param name="SourcePricedMarketValue">EN: Historical priced market value in source currency. FA: ارزش بازار تاریخی قیمت‌دار در ارز مبدا.</param>
/// <param name="SourcePricedNetAssetValue">EN: Historical priced NAV in source currency. FA: NAV تاریخی قیمت‌دار در ارز مبدا.</param>
/// <param name="SourceIsComplete">EN: True when historical source NAV is fully priced. FA: وقتی NAV تاریخی مبدا کاملاً قیمت‌گذاری شده باشد true است.</param>
/// <param name="SourceNetAssetValue">EN: Complete historical NAV in source currency, or null when incomplete. FA: NAV تاریخی کامل در ارز مبدا، یا null در صورت ناقص‌بودن.</param>
/// <param name="PricedPositionCount">EN: Number of historically priced open positions. FA: تعداد موقعیت‌های باز دارای قیمت تاریخی.</param>
/// <param name="UnpricedPositionCount">EN: Number of historically unpriced open positions. FA: تعداد موقعیت‌های باز فاقد قیمت تاریخی.</param>
/// <param name="IsFxAvailable">EN: True when an eligible historical FX conversion is available. FA: وقتی نرخ FX تاریخی معتبر موجود باشد true است.</param>
/// <param name="FxRateDate">EN: FX rate date used for translation. FA: تاریخ نرخ FX استفاده‌شده برای ترجمه.</param>
/// <param name="SourceToBaseRate">EN: Effective source-to-base conversion rate. FA: نرخ موثر تبدیل ارز مبدا به ارز پایه.</param>
/// <param name="CashBalanceBase">EN: Historical cash balance translated to base currency. FA: موجودی نقدی تاریخی ترجمه‌شده به ارز پایه.</param>
/// <param name="PricedMarketValueBase">EN: Historical priced market value translated to base currency. FA: ارزش بازار تاریخی قیمت‌دار ترجمه‌شده به ارز پایه.</param>
/// <param name="PricedNetAssetValueBase">EN: Historical priced NAV translated to base currency. FA: NAV تاریخی قیمت‌دار ترجمه‌شده به ارز پایه.</param>
/// <param name="NetAssetValueBase">EN: Complete historical NAV translated to base currency, or null when incomplete. FA: NAV تاریخی کامل ترجمه‌شده به ارز پایه، یا null در صورت ناقص‌بودن.</param>
public sealed record PortfolioTranslatedNavAsOfCurrencyResponse(
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
/// EN: Historical portfolio NAV translated into the configured base currency.
/// FA: NAV تاریخی پرتفوی پس از ترجمه به ارز پایه تنظیم‌شده.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Configured base currency identifier. FA: شناسه ارز پایه تنظیم‌شده.</param>
/// <param name="AsOf">EN: Historical cutoff instant used by the projection. FA: لحظه تاریخی استفاده‌شده در Projection.</param>
/// <param name="IsPricedNavFullyTranslated">EN: True when every source currency has an eligible historical FX rate. FA: وقتی برای تمام ارزهای مبدا نرخ FX تاریخی معتبر موجود باشد true است.</param>
/// <param name="IsComplete">EN: True when every source NAV is complete and every source currency is translated. FA: وقتی همه NAVهای مبدا کامل و همه ارزها قابل ترجمه باشند true است.</param>
/// <param name="PricedNetAssetValueBase">EN: Aggregate translated historical priced NAV, or null when FX translation is incomplete. FA: NAV تاریخی قیمت‌دار تجمیعی در ارز پایه، یا null در صورت ناقص‌بودن ترجمه FX.</param>
/// <param name="NetAssetValueBase">EN: Aggregate complete historical NAV in base currency, or null when pricing or FX is incomplete. FA: NAV تاریخی کامل تجمیعی در ارز پایه، یا null در صورت ناقص‌بودن قیمت‌گذاری یا FX.</param>
/// <param name="Currencies">EN: Translation rows by source currency. FA: ردیف‌های ترجمه به تفکیک ارز مبدا.</param>
public sealed record GetPortfolioTranslatedNavAsOfResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset AsOf,
    bool IsPricedNavFullyTranslated,
    bool IsComplete,
    decimal? PricedNetAssetValueBase,
    decimal? NetAssetValueBase,
    IReadOnlyCollection<PortfolioTranslatedNavAsOfCurrencyResponse> Currencies);
