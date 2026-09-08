// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks;

/// <summary>
/// EN: Item returned by an intraday-tick page.
/// FA: آیتم بازگشتی در صفحه Tickهای درون‌روزی.
/// </summary>
public sealed record IntradayPriceTickItem(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset OccurredAt,
    long SequenceNumber,
    decimal Price,
    long Volume,
    decimal TradeValue);

/// <summary>
/// EN: Paged response for intraday price ticks.
/// FA: پاسخ صفحه‌بندی‌شده Tickهای قیمت درون‌روزی.
/// </summary>
public sealed record GetIntradayPriceTicksResponse(
    IReadOnlyList<IntradayPriceTickItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
