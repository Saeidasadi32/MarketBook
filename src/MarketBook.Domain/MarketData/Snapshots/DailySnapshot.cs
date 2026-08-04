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
using MarketBook.Domain.MarketData.ValueObjects;

namespace MarketBook.Domain.MarketData.Snapshots;

/// <summary>
/// EN: Represents a complete end-of-day market snapshot.
/// FA: Snapshot کامل پایان روز بازار را نمایش می‌دهد.
/// </summary>
public sealed class DailySnapshot : IEquatable<DailySnapshot>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="DailySnapshot"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="DailySnapshot"/> را ایجاد می‌کند.
    /// </summary>
    public DailySnapshot(
        TradingDate tradingDate,
        DailyPrice dailyPrice,
        TradeStatistics statistics,
        OrderBook orderBook)
    {
        ArgumentNullException.ThrowIfNull(dailyPrice);
        ArgumentNullException.ThrowIfNull(statistics);
        ArgumentNullException.ThrowIfNull(orderBook);

        TradingDate = tradingDate;
        DailyPrice = dailyPrice;
        Statistics = statistics;
        OrderBook = orderBook;
        SnapshotTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Gets the trading date.
    /// FA: تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public TradingDate TradingDate { get; }

    /// <summary>
    /// EN: Gets the daily prices.
    /// FA: اطلاعات قیمت روزانه را دریافت می‌کند.
    /// </summary>
    public DailyPrice DailyPrice { get; }

    /// <summary>
    /// EN: Gets the trading statistics.
    /// FA: آمار معاملات را دریافت می‌کند.
    /// </summary>
    public TradeStatistics Statistics { get; }

    /// <summary>
    /// EN: Gets the order book.
    /// FA: دفتر سفارشات را دریافت می‌کند.
    /// </summary>
    public OrderBook OrderBook { get; }

    /// <summary>
    /// EN: Gets the snapshot creation time.
    /// FA: زمان ایجاد Snapshot را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset SnapshotTime { get; }

    /// <summary>
    /// EN: Determines whether the snapshot is valid.
    /// FA: مشخص می‌کند Snapshot معتبر است یا خیر.
    /// </summary>
    public bool IsValid()
    {
        return DailyPrice.ClosePrice.Value > 0 &&
               Statistics.Volume.Value > 0 &&
               Statistics.TradeCount > 0;
    }

    /// <summary>
    /// EN: Determines whether the specified snapshot is equal to the current snapshot.
    /// FA: تعیین می‌کند که Snapshot مشخص شده با Snapshot جاری برابر است یا خیر.
    /// </summary>
    public bool Equals(DailySnapshot? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return TradingDate == other.TradingDate &&
               DailyPrice == other.DailyPrice &&
               Statistics == other.Statistics &&
               OrderBook == other.OrderBook;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as DailySnapshot);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(TradingDate, DailyPrice, Statistics, OrderBook);

    /// <summary>
    /// EN: Returns a string representation of the snapshot.
    /// FA: نمایش رشته‌ای از Snapshot را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"DailySnapshot [{TradingDate}] - Close: {DailyPrice.ClosePrice}";
}
