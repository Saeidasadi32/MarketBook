// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Snapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.MarketData.Entities;

namespace MarketBook.Domain.MarketData.Snapshots;

/// <summary>
/// EN: Represents a complete end-of-day market snapshot.
/// FA: Snapshot کامل پایان روز بازار را نمایش می‌دهد.
/// </summary>
public sealed class DailySnapshot
{
    public DailySnapshot(
        DailyPrice dailyPrice,
        TradeStatistics statistics,
        OrderBook orderBook)
    {
        ArgumentNullException.ThrowIfNull(dailyPrice);
        ArgumentNullException.ThrowIfNull(statistics);
        ArgumentNullException.ThrowIfNull(orderBook);

        DailyPrice = dailyPrice;
        Statistics = statistics;
        OrderBook = orderBook;
    }

    /// <summary>
    /// EN: Daily prices.
    /// FA: اطلاعات قیمت روزانه.
    /// </summary>
    public DailyPrice DailyPrice { get; }

    /// <summary>
    /// EN: Trading statistics.
    /// FA: آمار معاملات.
    /// </summary>
    public TradeStatistics Statistics { get; }

    /// <summary>
    /// EN: Order book.
    /// FA: دفتر سفارشات.
    /// </summary>
    public OrderBook OrderBook { get; }
}