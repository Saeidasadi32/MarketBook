// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.ValueObjects;

namespace MarketBook.Domain.MarketData.Aggregates;

/// <summary>
/// EN: Represents one immutable executed-trade price tick for a Listing.
/// FA: یک Tick تغییرناپذیر از معامله انجام‌شده برای یک Listing را نمایش می‌دهد.
/// </summary>
public sealed class IntradayPriceTick : AggregateRoot<IntradayPriceTickId>
{
    /// <summary>
    /// EN: Initializes an intraday price tick.
    /// FA: یک Tick قیمت درون‌روزی را مقداردهی می‌کند.
    /// </summary>
    public IntradayPriceTick(
        IntradayPriceTickId id,
        ListingId listingId,
        DateOnly tradingDate,
        DateTimeOffset occurredAt,
        long sequenceNumber,
        decimal price,
        long volume)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(listingId);
        Validate(sequenceNumber, price, volume);

        ListingId = listingId;
        TradingDate = tradingDate;
        OccurredAt = occurredAt;
        SequenceNumber = sequenceNumber;
        Price = price;
        Volume = volume;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    private IntradayPriceTick()
    {
    }

    /// <summary>
    /// EN: Gets the Listing identifier.
    /// FA: شناسه Listing را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the logical trading date.
    /// FA: تاریخ معاملاتی منطقی را دریافت می‌کند.
    /// </summary>
    public DateOnly TradingDate { get; private set; }

    /// <summary>
    /// EN: Gets the exact tick timestamp including its offset.
    /// FA: زمان دقیق Tick را همراه با Offset دریافت می‌کند.
    /// </summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>
    /// EN: Gets the source sequence number within the trading date.
    /// FA: شماره توالی منبع در روز معاملاتی را دریافت می‌کند.
    /// </summary>
    public long SequenceNumber { get; private set; }

    /// <summary>
    /// EN: Gets the executed trade price.
    /// FA: قیمت معامله انجام‌شده را دریافت می‌کند.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// EN: Gets the executed trade volume.
    /// FA: حجم معامله انجام‌شده را دریافت می‌کند.
    /// </summary>
    public long Volume { get; private set; }

    /// <summary>
    /// EN: Gets the notional trade value calculated from price and volume.
    /// FA: ارزش اسمی معامله را از حاصل‌ضرب قیمت و حجم دریافت می‌کند.
    /// </summary>
    public decimal TradeValue => Price * Volume;

    /// <summary>
    /// EN: Gets persistence creation time.
    /// FA: زمان ایجاد رکورد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Creates an immutable intraday price tick.
    /// FA: یک Tick قیمت درون‌روزی تغییرناپذیر ایجاد می‌کند.
    /// </summary>
    public static IntradayPriceTick Create(
        ListingId listingId,
        DateOnly tradingDate,
        DateTimeOffset occurredAt,
        long sequenceNumber,
        decimal price,
        long volume)
        => new(
            IntradayPriceTickId.New(),
            listingId,
            tradingDate,
            occurredAt,
            sequenceNumber,
            price,
            volume);

    private static void Validate(
        long sequenceNumber,
        decimal price,
        long volume)
    {
        if (sequenceNumber < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sequenceNumber),
                "Sequence number cannot be negative.");

        if (price <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Trade price must be greater than zero.");

        if (volume <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(volume),
                "Trade volume must be greater than zero.");
    }
}
