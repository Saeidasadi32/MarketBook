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
/// EN: Represents the order book snapshot for a trading session.
/// FA: نمایی از دفتر سفارشات در یک لحظه معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed class OrderBook
{
    private readonly List<OrderBookLevel> _levels = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="OrderBook"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="OrderBook"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="levels">
    /// EN: Collection of order book levels.
    /// FA: مجموعه سطوح دفتر سفارشات.
    /// </param>
    public OrderBook(IEnumerable<OrderBookLevel> levels)
    {
        ArgumentNullException.ThrowIfNull(levels);

        _levels.AddRange(levels.OrderBy(x => x.Level));

        if (_levels.Count == 0)
            throw new ArgumentException(
                "Order book must contain at least one level.",
                nameof(levels));
    }

    /// <summary>
    /// EN: Gets all order book levels.
    /// FA: تمام سطوح دفتر سفارشات را دریافت می‌کند.
    /// </summary>
    public IReadOnlyList<OrderBookLevel> Levels => _levels;

    /// <summary>
    /// EN: Gets the best bid/ask level (Level 1).
    /// FA: بهترین سطح خرید و فروش (سطح اول) را دریافت می‌کند.
    /// </summary>
    public OrderBookLevel BestLevel => _levels[0];

    /// <summary>
    /// EN: Replaces the current order book snapshot.
    /// FA: Snapshot فعلی دفتر سفارشات را جایگزین می‌کند.
    /// </summary>
    /// <param name="levels">
    /// EN: New order book levels.
    /// FA: سطوح جدید دفتر سفارشات.
    /// </param>
    public void Replace(IEnumerable<OrderBookLevel> levels)
    {
        ArgumentNullException.ThrowIfNull(levels);

        _levels.Clear();
        _levels.AddRange(levels.OrderBy(x => x.Level));

        if (_levels.Count == 0)
            throw new InvalidOperationException(
                "Order book cannot be empty.");
    }
}