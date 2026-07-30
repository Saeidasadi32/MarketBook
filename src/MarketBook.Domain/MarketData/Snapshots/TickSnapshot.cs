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

namespace MarketBook.Domain.MarketData.Snapshots;

/// <summary>
/// EN: Represents a real-time tick snapshot (Level 1 market data).
/// FA: Snapshot لحظه‌ای Tick (داده‌های بازار سطح ۱) را نمایش می‌دهد.
/// </summary>
public sealed class TickSnapshot
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TickSnapshot"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TickSnapshot"/> را ایجاد می‌کند.
    /// </summary>
    public TickSnapshot(
        ListingId listingId,
        Price lastPrice,
        Price bidPrice,
        Price askPrice,
        Volume bidVolume,
        Volume askVolume,
        Volume lastVolume,
        DateTimeOffset timestamp)
    {
        ListingId = listingId;
        LastPrice = lastPrice;
        BidPrice = bidPrice;
        AskPrice = askPrice;
        BidVolume = bidVolume;
        AskVolume = askVolume;
        LastVolume = lastVolume;
        Timestamp = timestamp;
    }

    /// <summary>
    /// EN: Gets the listing identifier.
    /// FA: شناسه نماد را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Gets the last traded price.
    /// FA: آخرین قیمت معامله را دریافت می‌کند.
    /// </summary>
    public Price LastPrice { get; }

    /// <summary>
    /// EN: Gets the best bid price.
    /// FA: بهترین قیمت خرید را دریافت می‌کند.
    /// </summary>
    public Price BidPrice { get; }

    /// <summary>
    /// EN: Gets the best ask price.
    /// FA: بهترین قیمت فروش را دریافت می‌کند.
    /// </summary>
    public Price AskPrice { get; }

    /// <summary>
    /// EN: Gets the bid volume.
    /// FA: حجم خرید را دریافت می‌کند.
    /// </summary>
    public Volume BidVolume { get; }

    /// <summary>
    /// EN: Gets the ask volume.
    /// FA: حجم فروش را دریافت می‌کند.
    /// </summary>
    public Volume AskVolume { get; }

    /// <summary>
    /// EN: Gets the last trade volume.
    /// FA: حجم آخرین معامله را دریافت می‌کند.
    /// </summary>
    public Volume LastVolume { get; }

    /// <summary>
    /// EN: Gets the timestamp of the snapshot.
    /// FA: زمان Snapshot را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// EN: Gets the bid-ask spread.
    /// FA: اختلاف قیمت خرید و فروش را دریافت می‌کند.
    /// </summary>
    public Price Spread => new(AskPrice.Value - BidPrice.Value);

    /// <summary>
    /// EN: Calculates the mid price (average of bid and ask).
    /// FA: قیمت میانی (میانگین خرید و فروش) را محاسبه می‌کند.
    /// </summary>
    public Price MidPrice => new((BidPrice.Value + AskPrice.Value) / 2);

    /// <summary>
    /// EN: Determines whether the tick is an uptick (price increased).
    /// FA: مشخص می‌کند که Tick صعودی است یا خیر.
    /// </summary>
    public bool IsUptick { get; init; }

    /// <summary>
    /// EN: Determines whether the tick is a downtick (price decreased).
    /// FA: مشخص می‌کند که Tick نزولی است یا خیر.
    /// </summary>
    public bool IsDowntick { get; init; }

    /// <summary>
    /// EN: Returns a string representation of the tick snapshot.
    /// FA: نمایش رشته‌ای از Snapshot Tick را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"Tick: {ListingId} @ {LastPrice} | Bid: {BidPrice} ({BidVolume}) | Ask: {AskPrice} ({AskVolume}) | {Timestamp:HH:mm:ss.fff}";
}