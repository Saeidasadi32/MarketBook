// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;

/// <summary>
/// EN: NAV projection for one currency without cross-currency aggregation.
/// FA: تصویر NAV برای یک ارز بدون تجمیع بین ارزهای مختلف.
/// </summary>
/// <param name="CurrencyId">EN: Currency identifier. FA: شناسه ارز.</param>
/// <param name="CashBalance">EN: Cash-ledger balance after all credits/debits and trade settlements. FA: موجودی دفتر نقدی پس از همه بستانکاری/بدهکاری‌ها و تسویه معاملات.</param>
/// <param name="PricedMarketValue">EN: Market value of open positions that have a price. FA: ارزش بازار موقعیت‌های بازی که قیمت دارند.</param>
/// <param name="PricedNetAssetValue">EN: Cash plus priced market value; this remains calculable when some positions are unpriced. FA: وجه نقد به‌علاوه ارزش بازار موقعیت‌های قیمت‌دار؛ حتی در وجود موقعیت بدون قیمت قابل محاسبه است.</param>
/// <param name="IsComplete">EN: True when every open position in the currency is priced. FA: وقتی همه موقعیت‌های باز آن ارز قیمت دارند true است.</param>
/// <param name="NetAssetValue">EN: Complete NAV, or null when any open position is unpriced. FA: NAV کامل، یا null اگر هر موقعیت بازی بدون قیمت باشد.</param>
/// <param name="PricedPositionCount">EN: Number of priced open positions. FA: تعداد موقعیت‌های باز قیمت‌دار.</param>
/// <param name="UnpricedPositionCount">EN: Number of unpriced open positions. FA: تعداد موقعیت‌های باز بدون قیمت.</param>
public sealed record PortfolioNavCurrencyResponse(
    string CurrencyId,
    decimal CashBalance,
    decimal PricedMarketValue,
    decimal PricedNetAssetValue,
    bool IsComplete,
    decimal? NetAssetValue,
    int PricedPositionCount,
    int UnpricedPositionCount);

/// <summary>
/// EN: Current portfolio NAV grouped by currency. Monetary amounts are never summed across different currencies.
/// FA: NAV جاری پرتفوی به تفکیک ارز. مبالغ ارزهای متفاوت هرگز مستقیماً با هم جمع نمی‌شوند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="Currencies">EN: Per-currency NAV rows. FA: ردیف‌های NAV به تفکیک ارز.</param>
public sealed record GetPortfolioNavResponse(
    string PortfolioId,
    IReadOnlyCollection<PortfolioNavCurrencyResponse> Currencies);
