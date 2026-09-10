// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;

/// <summary>
/// EN: Historical NAV detail for one currency.
/// FA: جزئیات NAV تاریخی برای یک ارز.
/// </summary>
/// <param name="CurrencyId">EN: Currency identifier. FA: شناسه ارز.</param>
/// <param name="CashBalance">EN: Cash balance as of the cutoff. FA: موجودی نقدی در لحظه برش.</param>
/// <param name="PricedMarketValue">EN: Market value of positions having an eligible historical price. FA: ارزش بازار موقعیت‌هایی که قیمت تاریخی مجاز دارند.</param>
/// <param name="PricedNetAssetValue">EN: Cash plus priced market value. FA: وجه نقد به‌علاوه ارزش بازار قیمت‌دار.</param>
/// <param name="IsComplete">EN: True when every open position has a historical price on or before the cutoff date. FA: وقتی همه موقعیت‌های باز قیمت تاریخی در تاریخ برش یا قبل از آن دارند true است.</param>
/// <param name="NetAssetValue">EN: Complete historical NAV; null when pricing is incomplete. FA: NAV تاریخی کامل؛ در صورت ناقص‌بودن قیمت‌گذاری null است.</param>
/// <param name="PricedPositionCount">EN: Number of priced open positions. FA: تعداد موقعیت‌های باز قیمت‌دار.</param>
/// <param name="UnpricedPositionCount">EN: Number of unpriced open positions. FA: تعداد موقعیت‌های باز بدون قیمت.</param>
public sealed record PortfolioNavAsOfCurrencyResponse(
    string CurrencyId,
    decimal CashBalance,
    decimal PricedMarketValue,
    decimal PricedNetAssetValue,
    bool IsComplete,
    decimal? NetAssetValue,
    int PricedPositionCount,
    int UnpricedPositionCount);

/// <summary>
/// EN: Portfolio NAV reconstructed at a historical cutoff instant.
/// FA: NAV پرتفوی بازسازی‌شده در یک لحظه تاریخی مشخص.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="AsOf">EN: Inclusive historical cutoff instant used by the projection. FA: لحظه تاریخی شامل‌شونده استفاده‌شده در Projection.</param>
/// <param name="Currencies">EN: Historical NAV rows by currency. FA: ردیف‌های NAV تاریخی به تفکیک ارز.</param>
public sealed record GetPortfolioNavAsOfResponse(
    string PortfolioId,
    DateTimeOffset AsOf,
    IReadOnlyCollection<PortfolioNavAsOfCurrencyResponse> Currencies);
