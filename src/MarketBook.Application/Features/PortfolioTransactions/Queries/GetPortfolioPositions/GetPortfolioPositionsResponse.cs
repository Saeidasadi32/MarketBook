// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions;

/// <summary>
/// EN: Represents one projected open portfolio position.
/// FA: یک موقعیت باز محاسبه‌شده پرتفوی را نمایش می‌دهد.
/// </summary>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه Listing.</param>
/// <param name="CurrencyId">EN: Transaction quote-currency identifier. FA: شناسه ارز مظنه تراکنش.</param>
/// <param name="Quantity">EN: Current open quantity. FA: تعداد باز فعلی.</param>
/// <param name="AverageAcquisitionPrice">EN: Weighted average acquisition price. FA: میانگین موزون قیمت خرید.</param>
/// <param name="InvestedCost">EN: Remaining book cost. FA: بهای تمام‌شده دفتری باقیمانده.</param>
public sealed record PortfolioPositionResponse(
    string ListingId,
    string CurrencyId,
    decimal Quantity,
    decimal AverageAcquisitionPrice,
    decimal InvestedCost);

/// <summary>
/// EN: Represents the current position projection result.
/// FA: نتیجه Projection موقعیت‌های فعلی را نمایش می‌دهد.
/// </summary>
/// <param name="Items">EN: Open positions. FA: موقعیت‌های باز.</param>
public sealed record GetPortfolioPositionsResponse(
    IReadOnlyCollection<PortfolioPositionResponse> Items);
