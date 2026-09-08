// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl;

/// <summary>
/// EN: Total P/L details for one Listing.
/// FA: جزئیات سود/زیان کل برای یک لیستینگ.
/// </summary>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه لیستینگ.</param>
/// <param name="CurrencyId">EN: Transaction and quote currency identifier. FA: شناسه ارز تراکنش و مظنه.</param>
/// <param name="RealizedProfitLoss">EN: Realized P/L from completed sale quantity. FA: سود/زیان تحقق‌یافته از مقدار فروخته‌شده.</param>
/// <param name="OpenQuantity">EN: Remaining open quantity. FA: مقدار باز باقی‌مانده.</param>
/// <param name="RemainingBookCost">EN: Remaining acquisition book cost. FA: بهای دفتری خرید باقی‌مانده.</param>
/// <param name="IsPriced">EN: Indicates whether an open position has a market price, or is closed. FA: مشخص می‌کند موقعیت باز قیمت بازار دارد یا موقعیت بسته است.</param>
/// <param name="MarketPrice">EN: Latest market price for an open position. FA: آخرین قیمت بازار برای موقعیت باز.</param>
/// <param name="MarketValue">EN: Current market value of the open position. FA: ارزش بازار جاری موقعیت باز.</param>
/// <param name="UnrealizedProfitLoss">EN: Unrealized P/L; zero for a closed position and null for an unpriced open position. FA: سود/زیان تحقق‌نیافته؛ برای موقعیت بسته صفر و برای موقعیت باز بدون قیمت null است.</param>
/// <param name="TotalProfitLoss">EN: Realized plus unrealized P/L; null when an open position is unpriced. FA: مجموع سود/زیان تحقق‌یافته و تحقق‌نیافته؛ برای موقعیت باز بدون قیمت null است.</param>
public sealed record PortfolioTotalPnlItemResponse(
    string ListingId,
    string CurrencyId,
    decimal RealizedProfitLoss,
    decimal OpenQuantity,
    decimal RemainingBookCost,
    bool IsPriced,
    decimal? MarketPrice,
    decimal? MarketValue,
    decimal? UnrealizedProfitLoss,
    decimal? TotalProfitLoss);

/// <summary>
/// EN: Currency-level P/L summary. It never combines different currencies.
/// FA: خلاصه سود/زیان در سطح ارز که هرگز ارزهای مختلف را با یکدیگر ترکیب نمی‌کند.
/// </summary>
/// <param name="CurrencyId">EN: Currency identifier. FA: شناسه ارز.</param>
/// <param name="IsFullyPriced">EN: Indicates whether every open position in this currency has a market price. FA: مشخص می‌کند همه موقعیت‌های باز این ارز دارای قیمت بازار هستند یا خیر.</param>
/// <param name="RealizedProfitLoss">EN: Sum of realized P/L in this currency. FA: مجموع سود/زیان تحقق‌یافته در این ارز.</param>
/// <param name="UnrealizedProfitLoss">EN: Sum of unrealized P/L when fully priced; otherwise null. FA: مجموع سود/زیان تحقق‌نیافته در صورت کامل بودن قیمت‌ها؛ در غیر این صورت null.</param>
/// <param name="TotalProfitLoss">EN: Realized plus unrealized P/L when fully priced; otherwise null. FA: مجموع سود/زیان تحقق‌یافته و تحقق‌نیافته در صورت کامل بودن قیمت‌ها؛ در غیر این صورت null.</param>
public sealed record PortfolioTotalPnlCurrencyResponse(
    string CurrencyId,
    bool IsFullyPriced,
    decimal RealizedProfitLoss,
    decimal? UnrealizedProfitLoss,
    decimal? TotalProfitLoss);

/// <summary>
/// EN: Portfolio total P/L response, separated by currency.
/// FA: پاسخ سود/زیان کل پرتفوی به تفکیک ارز.
/// </summary>
/// <param name="Currencies">EN: Currency summaries. FA: خلاصه‌های ارزی.</param>
/// <param name="Items">EN: Per-listing P/L details. FA: جزئیات سود/زیان به تفکیک لیستینگ.</param>
public sealed record GetPortfolioTotalPnlResponse(
    IReadOnlyCollection<PortfolioTotalPnlCurrencyResponse> Currencies,
    IReadOnlyCollection<PortfolioTotalPnlItemResponse> Items);
