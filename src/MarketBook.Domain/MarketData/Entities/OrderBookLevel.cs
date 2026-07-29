// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;

namespace MarketBook.Domain.MarketData.Entities;

/// <summary>
/// EN: Represents one level of the order book.
/// FA: یک سطح از دفتر سفارشات را نمایش می‌دهد.
/// </summary>
public sealed class OrderBookLevel
{
    /// <summary>
    /// EN: Initializes a new order book level.
    /// FA: یک سطح جدید از دفتر سفارشات ایجاد می‌کند.
    /// </summary>
    public OrderBookLevel(
        int level,
        Price bidPrice,
        Volume bidVolume,
        int bidCount,
        Price askPrice,
        Volume askVolume,
        int askCount)
    {
        if (level <= 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        if (bidCount < 0)
            throw new ArgumentOutOfRangeException(nameof(bidCount));

        if (askCount < 0)
            throw new ArgumentOutOfRangeException(nameof(askCount));

        Level = level;
        BidPrice = bidPrice;
        BidVolume = bidVolume;
        BidCount = bidCount;
        AskPrice = askPrice;
        AskVolume = askVolume;
        AskCount = askCount;
    }

    /// <summary>
    /// EN: Gets order book level.
    /// FA: شماره سطح دفتر سفارشات.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// EN: Gets best bid price.
    /// FA: قیمت خرید.
    /// </summary>
    public Price BidPrice { get; }

    /// <summary>
    /// EN: Gets bid volume.
    /// FA: حجم خرید.
    /// </summary>
    public Volume BidVolume { get; }

    /// <summary>
    /// EN: Gets bid order count.
    /// FA: تعداد سفارش‌های خرید.
    /// </summary>
    public int BidCount { get; }

    /// <summary>
    /// EN: Gets best ask price.
    /// FA: قیمت فروش.
    /// </summary>
    public Price AskPrice { get; }

    /// <summary>
    /// EN: Gets ask volume.
    /// FA: حجم فروش.
    /// </summary>
    public Volume AskVolume { get; }

    /// <summary>
    /// EN: Gets ask order count.
    /// FA: تعداد سفارش‌های فروش.
    /// </summary>
    public int AskCount { get; }
}