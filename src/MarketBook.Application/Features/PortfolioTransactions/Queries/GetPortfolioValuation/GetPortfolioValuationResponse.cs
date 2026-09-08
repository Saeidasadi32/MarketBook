// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation;

/// <summary>
/// EN: Current valuation for one open portfolio position.
/// FA: ارزش‌گذاری جاری یک موقعیت باز پرتفوی.
/// </summary>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه لیستینگ.</param>
/// <param name="CurrencyId">EN: Position and quote currency identifier. FA: شناسه ارز موقعیت و ارز مظنه.</param>
/// <param name="Quantity">EN: Open quantity. FA: مقدار باز.</param>
/// <param name="AverageAcquisitionPrice">EN: Weighted-average acquisition price including buy costs. FA: میانگین موزون بهای خرید با احتساب هزینه‌های خرید.</param>
/// <param name="InvestedCost">EN: Remaining acquisition book cost. FA: بهای دفتری خرید باقی‌مانده.</param>
/// <param name="IsPriced">EN: Indicates whether a market price is available. FA: مشخص می‌کند قیمت بازار موجود است یا خیر.</param>
/// <param name="MarketPriceDate">EN: Trading date of the applied market price. FA: تاریخ معاملاتی قیمت بازار اعمال‌شده.</param>
/// <param name="MarketPrice">EN: Latest available last-traded price. FA: آخرین قیمت معامله‌شده موجود.</param>
/// <param name="MarketValue">EN: Quantity multiplied by market price. FA: مقدار ضرب‌در قیمت بازار.</param>
/// <param name="UnrealizedProfitLoss">EN: Market value less invested cost. FA: ارزش بازار منهای بهای سرمایه‌گذاری‌شده.</param>
/// <param name="UnrealizedReturnPercent">EN: Unrealized return percentage relative to invested cost. FA: درصد بازده تحقق‌نیافته نسبت به بهای سرمایه‌گذاری‌شده.</param>
public sealed record PortfolioValuationItemResponse(
    string ListingId,
    string CurrencyId,
    decimal Quantity,
    decimal AverageAcquisitionPrice,
    decimal InvestedCost,
    bool IsPriced,
    DateOnly? MarketPriceDate,
    decimal? MarketPrice,
    decimal? MarketValue,
    decimal? UnrealizedProfitLoss,
    decimal? UnrealizedReturnPercent);

/// <summary>
/// EN: Current portfolio valuation response. Monetary values are not aggregated across currencies.
/// FA: پاسخ ارزش‌گذاری جاری پرتفوی. مقادیر پولی بین ارزهای مختلف با یکدیگر تجمیع نمی‌شوند.
/// </summary>
/// <param name="Items">EN: Per-listing valuation items. FA: اقلام ارزش‌گذاری به تفکیک لیستینگ.</param>
public sealed record GetPortfolioValuationResponse(
    IReadOnlyCollection<PortfolioValuationItemResponse> Items);
