// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Snapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Entities;

namespace MarketBook.Domain.MarketData.Snapshots;

/// <summary>
/// EN: Represents an intraday market snapshot at a specific time.
/// FA: Snapshot درون‌روز بازار در یک زمان مشخص را نمایش می‌دهد.
/// </summary>
public sealed class IntradaySnapshot
{
    private readonly List<TickSnapshot> _ticks = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="IntradaySnapshot"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="IntradaySnapshot"/> را ایجاد می‌کند.
    /// </summary>
    public IntradaySnapshot(
        ListingId listingId,
        DateTimeOffset snapshotTime,
        Price openPrice,
        Price highPrice,
        Price lowPrice,
        Price lastPrice,
        Volume cumulativeVolume,
        TradeCount cumulativeTradeCount,
        Money cumulativeTradeValue,
        TickSnapshot? lastTick = null)
    {
        ListingId = listingId;
        SnapshotTime = snapshotTime;
        OpenPrice = openPrice;
        HighPrice = highPrice;
        LowPrice = lowPrice;
        LastPrice = lastPrice;
        CumulativeVolume = cumulativeVolume;
        CumulativeTradeCount = cumulativeTradeCount;
        CumulativeTradeValue = cumulativeTradeValue;
        LastTick = lastTick;
    }

    /// <summary>
    /// EN: Gets the listing identifier.
    /// FA: شناسه نماد را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Gets the snapshot time.
    /// FA: زمان Snapshot را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset SnapshotTime { get; }

    /// <summary>
    /// EN: Gets the opening price.
    /// FA: قیمت آغازین را دریافت می‌کند.
    /// </summary>
    public Price OpenPrice { get; }

    /// <summary>
    /// EN: Gets the highest price so far.
    /// FA: بیشترین قیمت تاکنون را دریافت می‌کند.
    /// </summary>
    public Price HighPrice { get; }

    /// <summary>
    /// EN: Gets the lowest price so far.
    /// FA: کمترین قیمت تاکنون را دریافت می‌کند.
    /// </summary>
    public Price LowPrice { get; }

    /// <summary>
    /// EN: Gets the last traded price.
    /// FA: آخرین قیمت معامله را دریافت می‌کند.
    /// </summary>
    public Price LastPrice { get; }

    /// <summary>
    /// EN: Gets the cumulative volume.
    /// FA: حجم تجمعی را دریافت می‌کند.
    /// </summary>
    public Volume CumulativeVolume { get; }

    /// <summary>
    /// EN: Gets the cumulative trade count.
    /// FA: تعداد معاملات تجمعی را دریافت می‌کند.
    /// </summary>
    public TradeCount CumulativeTradeCount { get; }

    /// <summary>
    /// EN: Gets the cumulative trade value.
    /// FA: ارزش معاملات تجمعی را دریافت می‌کند.
    /// </summary>
    public Money CumulativeTradeValue { get; }

    /// <summary>
    /// EN: Gets the last tick snapshot.
    /// FA: آخرین Snapshot Tick را دریافت می‌کند.
    /// </summary>
    public TickSnapshot? LastTick { get; }

    /// <summary>
    /// EN: Gets the price change from open.
    /// FA: تغییر قیمت از ابتدا را دریافت می‌کند.
    /// </summary>
    public Price ChangeFromOpen => new(LastPrice.Value - OpenPrice.Value);

    /// <summary>
    /// EN: Gets the percentage change from open.
    /// FA: درصد تغییر از ابتدا را دریافت می‌کند.
    /// </summary>
    public PercentageChange ChangeFromOpenPercentage {
        get {
            if (OpenPrice.Value == 0)
                return PercentageChange.Zero;

            return PercentageChange.Calculate(OpenPrice.Value, LastPrice.Value);
        }
    }

    /// <summary>
    /// EN: Gets the daily range (high - low).
    /// FA: محدوده روزانه (بیشترین - کمترین) را دریافت می‌کند.
    /// </summary>
    public Price Range => new(HighPrice.Value - LowPrice.Value);

    /// <summary>
    /// EN: Gets all tick snapshots in this intraday session.
    /// FA: تمام Snapshot‌های Tick در این جلسه درون‌روز را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<TickSnapshot> Ticks => _ticks.AsReadOnly();

    /// <summary>
    /// EN: Adds a tick snapshot to the intraday session.
    /// FA: یک Snapshot Tick به جلسه درون‌روز اضافه می‌کند.
    /// </summary>
    public void AddTick(TickSnapshot tick)
    {
        ArgumentNullException.ThrowIfNull(tick);

        if (tick.ListingId != ListingId)
            throw new InvalidOperationException("Tick does not belong to this listing.");

        _ticks.Add(tick);
    }

    /// <summary>
    /// EN: Updates the intraday snapshot with a new tick.
    /// FA: Snapshot درون‌روز را با یک Tick جدید به‌روزرسانی می‌کند.
    /// </summary>
    public IntradaySnapshot Update(TickSnapshot tick)
    {
        ArgumentNullException.ThrowIfNull(tick);

        if (tick.ListingId != ListingId)
            throw new InvalidOperationException("Tick does not belong to this listing.");

        var newHigh = tick.LastPrice.Value > HighPrice.Value ? tick.LastPrice : HighPrice;
        var newLow = tick.LastPrice.Value < LowPrice.Value ? tick.LastPrice : LowPrice;

        AddTick(tick);

        return new IntradaySnapshot(
            ListingId,
            tick.Timestamp,
            OpenPrice,
            newHigh,
            newLow,
            tick.LastPrice,
            new Volume(CumulativeVolume.Value + tick.LastVolume.Value),
            new TradeCount(CumulativeTradeCount.Value + 1),
            new Money(CumulativeTradeValue.Value + (tick.LastPrice.Value * tick.LastVolume.Value)),
            tick);
    }

    /// <summary>
    /// EN: Returns a string representation of the intraday snapshot.
    /// FA: نمایش رشته‌ای از Snapshot درون‌روز را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"Intraday: {ListingId} @ {SnapshotTime:HH:mm} | O: {OpenPrice}, H: {HighPrice}, L: {LowPrice}, C: {LastPrice}, V: {CumulativeVolume}";
}