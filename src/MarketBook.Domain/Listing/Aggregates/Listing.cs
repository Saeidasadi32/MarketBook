// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Listing.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Domain.Venue.ValueObjects;

namespace MarketBook.Domain.Listing.Aggregates;

/// <summary>
/// EN: Represents a tradable listing of an instrument on a market.
/// FA: نمایش‌دهنده پذیرش یک ابزار مالی در یک بازار مشخص است.
/// </summary>
public sealed class Listing : AggregateRoot<ListingId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Listing"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Listing"/> را ایجاد می‌کند.
    /// </summary>
    public Listing(
        ListingId id,
        InstrumentId instrumentId,
        VenueId venueId,
        CurrencyId currencyId,
        TradingSymbol tradingSymbol,
        decimal tickSize,
        byte pricePrecision)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(instrumentId);
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(currencyId);
        ArgumentNullException.ThrowIfNull(tradingSymbol);

        if (tickSize <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(tickSize),
                "Tick size must be greater than zero.");

        InstrumentId = instrumentId;
        VenueId = venueId;
        CurrencyId = currencyId;
        TradingSymbol = tradingSymbol;
        TickSize = tickSize;
        PricePrecision = pricePrecision;

        IsPrimary = false;
        IsActive = true;
        CreatedOn = DateTimeOffset.UtcNow;
        ActivatedOn = CreatedOn;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Listing()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the related instrument identifier.
    /// FA: شناسه ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentId InstrumentId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the concrete trading venue.
    /// FA: بستر مشخص اجرای معامله.
    /// </summary>
    public VenueId VenueId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the trading symbol.
    /// FA: نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public TradingSymbol TradingSymbol { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the currency identifier.
    /// FA: شناسه ارز را دریافت می‌کند.
    /// </summary>
    public CurrencyId CurrencyId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the minimum price increment.
    /// FA: حداقل گام تغییر قیمت را دریافت می‌کند.
    /// </summary>
    public decimal TickSize { get; }

    /// <summary>
    /// EN: Gets the supported price precision.
    /// FA: دقت قیمت را دریافت می‌کند.
    /// </summary>
    public byte PricePrecision { get; }

    /// <summary>
    /// EN: Gets whether this is the primary listing.
    /// FA: مشخص می‌کند این پذیرش، پذیرش اصلی است یا خیر.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; } = default!;

    /// <summary>
    /// EN: Gets whether the listing is active.
    /// FA: مشخص می‌کند پذیرش فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets the timestamp when the listing became active.
    /// FA: زمان فعال شدن پذیرش معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset? ActivatedOn { get; private set; }

    /// <summary>
    /// EN: Gets the timestamp when the listing became inactive.
    /// FA: زمان غیرفعال شدن پذیرش معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset? DeactivatedOn { get; private set; }

    /// <summary>
    /// EN: Marks this listing as the primary listing.
    /// FA: این پذیرش را به عنوان پذیرش اصلی تعیین می‌کند.
    /// </summary>
    public void MakePrimary()
    {
        if (IsPrimary)
            return;

        IsPrimary = true;
        Raise(new ListingMadePrimaryEvent(Id));
    }

    /// <summary>
    /// EN: Removes the primary flag.
    /// FA: وضعیت پذیرش اصلی را حذف می‌کند.
    /// </summary>
    public void RemovePrimary()
    {
        if (!IsPrimary)
            return;

        IsPrimary = false;
        Raise(new ListingPrimaryRemovedEvent(Id));
    }

    /// <summary>
    /// EN: Activates the listing.
    /// FA: پذیرش را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        ActivatedOn = DateTimeOffset.UtcNow;
        DeactivatedOn = null;

        Raise(new ListingActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the listing.
    /// FA: پذیرش را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        DeactivatedOn = DateTimeOffset.UtcNow;

        Raise(new ListingDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Rounds a price to the listing's precision and tick size.
    /// FA: قیمت را بر اساس دقت و گام تغییر قیمت گرد می‌کند.
    /// </summary>
    public decimal RoundPrice(decimal price)
    {
        decimal rounded = Math.Round(price / TickSize) * TickSize;
        return Math.Round(rounded, PricePrecision, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instrumentId"></param>
    /// <param name="venueId"></param>
    /// <param name="currencyId"></param>
    /// <param name="tradingSymbol"></param>
    /// <param name="tickSize"></param>
    /// <param name="pricePrecision"></param>
    /// <returns></returns>
    public static Listing Create(
        InstrumentId instrumentId,
        VenueId venueId,
        CurrencyId currencyId,
        TradingSymbol tradingSymbol,
        decimal tickSize,
        byte pricePrecision)
    {
        return new Listing(
            ListingId.New(),
            instrumentId,
            venueId,
            currencyId,
            tradingSymbol,
            tickSize,
            pricePrecision);
    }
}

/// <summary>
/// EN: Domain event raised when a listing becomes primary.
/// FA: رویداد دامنه زمانی که پذیرش به اصلی تبدیل می‌شود.
/// </summary>
public sealed record ListingMadePrimaryEvent(ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when primary flag is removed.
/// FA: رویداد دامنه زمانی که وضعیت اصلی حذف می‌شود.
/// </summary>
public sealed record ListingPrimaryRemovedEvent(ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a listing is activated.
/// FA: رویداد دامنه زمانی که پذیرش فعال می‌شود.
/// </summary>
public sealed record ListingActivatedEvent(ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a listing is deactivated.
/// FA: رویداد دامنه زمانی که پذیرش غیرفعال می‌شود.
/// </summary>
public sealed record ListingDeactivatedEvent(ListingId ListingId) : DomainEvent;
