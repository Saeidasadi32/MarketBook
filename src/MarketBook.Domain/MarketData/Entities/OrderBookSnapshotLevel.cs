// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.MarketData.Entities;

/// <summary>
/// EN: Represents one ranked bid/ask level inside an order-book snapshot.
/// FA: یک سطح رتبه‌بندی‌شده خرید/فروش در Snapshot دفتر سفارشات را نمایش می‌دهد.
/// </summary>
public sealed class OrderBookSnapshotLevel
{
    /// <summary>
    /// EN: Initializes one order-book snapshot level.
    /// FA: یک سطح از Snapshot دفتر سفارشات را مقداردهی می‌کند.
    /// </summary>
    public OrderBookSnapshotLevel(
        int level,
        decimal? bidPrice,
        long? bidVolume,
        int? bidOrderCount,
        decimal? askPrice,
        long? askVolume,
        int? askOrderCount)
    {
        ValidateSide(
            "bid",
            bidPrice,
            bidVolume,
            bidOrderCount);
        ValidateSide(
            "ask",
            askPrice,
            askVolume,
            askOrderCount);

        if (level < 1)
            throw new ArgumentOutOfRangeException(
                nameof(level),
                "Order-book level must be greater than zero.");

        if (bidPrice is null && askPrice is null)
            throw new ArgumentException(
                "At least one side of an order-book level must be populated.");

        Level = level;
        BidPrice = bidPrice;
        BidVolume = bidVolume;
        BidOrderCount = bidOrderCount;
        AskPrice = askPrice;
        AskVolume = askVolume;
        AskOrderCount = askOrderCount;
    }

    private OrderBookSnapshotLevel()
    {
    }

    /// <summary>EN: Gets the one-based depth level. FA: شماره سطح یک‌مبنایی عمق بازار را دریافت می‌کند.</summary>
    public int Level { get; private set; }

    /// <summary>EN: Gets the bid price, when present. FA: قیمت خرید را در صورت وجود دریافت می‌کند.</summary>
    public decimal? BidPrice { get; private set; }

    /// <summary>EN: Gets the bid volume, when present. FA: حجم خرید را در صورت وجود دریافت می‌کند.</summary>
    public long? BidVolume { get; private set; }

    /// <summary>EN: Gets the bid order count, when present. FA: تعداد سفارش خرید را در صورت وجود دریافت می‌کند.</summary>
    public int? BidOrderCount { get; private set; }

    /// <summary>EN: Gets the ask price, when present. FA: قیمت فروش را در صورت وجود دریافت می‌کند.</summary>
    public decimal? AskPrice { get; private set; }

    /// <summary>EN: Gets the ask volume, when present. FA: حجم فروش را در صورت وجود دریافت می‌کند.</summary>
    public long? AskVolume { get; private set; }

    /// <summary>EN: Gets the ask order count, when present. FA: تعداد سفارش فروش را در صورت وجود دریافت می‌کند.</summary>
    public int? AskOrderCount { get; private set; }

    private static void ValidateSide(
        string side,
        decimal? price,
        long? volume,
        int? orderCount)
    {
        bool any = price.HasValue || volume.HasValue || orderCount.HasValue;
        bool all = price.HasValue && volume.HasValue && orderCount.HasValue;

        if (any && !all)
            throw new ArgumentException(
                $"The {side} side must provide price, volume, and order count together.");

        if (!all)
            return;

        if (price!.Value <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(price),
                $"The {side} price must be greater than zero.");

        if (volume!.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(volume),
                $"The {side} volume cannot be negative.");

        if (orderCount!.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(orderCount),
                $"The {side} order count cannot be negative.");
    }
}
