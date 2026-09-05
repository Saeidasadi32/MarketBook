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
using MarketBook.Domain.Venue.ValueObjects;

namespace MarketBook.Domain.Listing.Aggregates;

/// <summary>
/// EN: Represents a tradable listing of an instrument on a concrete venue.
/// FA: یک پذیرش قابل معامله از ابزار مالی را در یک Venue مشخص نمایش می‌دهد.
/// </summary>
public sealed class Listing : AggregateRoot<ListingId>
{
    /// <summary>
    /// EN: Initializes a listing with its immutable identity links and pricing attributes.
    /// FA: Listing را با ارتباطات هویتی ثابت و ویژگی‌های قیمت‌گذاری مقداردهی می‌کند.
    /// </summary>
    public Listing(
        ListingId id,
        InstrumentId instrumentId,
        VenueId venueId,
        CurrencyId quoteCurrencyId,
        TradingSymbol tradingSymbol,
        decimal tickSize,
        byte pricePrecision)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(instrumentId);
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(quoteCurrencyId);
        ArgumentNullException.ThrowIfNull(tradingSymbol);

        ValidatePricing(tickSize, pricePrecision);

        InstrumentId = instrumentId;
        VenueId = venueId;
        QuoteCurrencyId = quoteCurrencyId;
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
    }

    /// <summary>
    /// EN: Gets the instrument identifier. This link is immutable after creation.
    /// FA: شناسه ابزار مالی را دریافت می‌کند؛ این ارتباط پس از ایجاد ثابت است.
    /// </summary>
    public InstrumentId InstrumentId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the venue identifier. This link is immutable after creation.
    /// FA: شناسه Venue را دریافت می‌کند؛ این ارتباط پس از ایجاد ثابت است.
    /// </summary>
    public VenueId VenueId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the trading symbol.
    /// FA: نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public TradingSymbol TradingSymbol { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the quote currency used for listing prices.
    /// FA: ارز مظنه مورد استفاده برای قیمت‌های Listing را دریافت می‌کند.
    /// </summary>
    public CurrencyId QuoteCurrencyId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the minimum price increment.
    /// FA: حداقل گام تغییر قیمت را دریافت می‌کند.
    /// </summary>
    public decimal TickSize { get; private set; }

    /// <summary>
    /// EN: Gets the supported price precision.
    /// FA: دقت قیمت را دریافت می‌کند.
    /// </summary>
    public byte PricePrecision { get; private set; }

    /// <summary>
    /// EN: Gets whether this is the primary listing of the instrument.
    /// FA: مشخص می‌کند آیا این Listing پذیرش اصلی Instrument است یا خیر.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp.
    /// FA: زمان ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether the listing is active.
    /// FA: مشخص می‌کند Listing فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets the most recent activation timestamp.
    /// FA: آخرین زمان فعال شدن Listing را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset? ActivatedOn { get; private set; }

    /// <summary>
    /// EN: Gets the most recent deactivation timestamp.
    /// FA: آخرین زمان غیرفعال شدن Listing را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset? DeactivatedOn { get; private set; }

    /// <summary>
    /// EN: Changes the venue-scoped trading symbol.
    /// FA: نماد معاملاتی Listing را در محدوده Venue تغییر می‌دهد.
    /// </summary>
    public void ChangeTradingSymbol(TradingSymbol tradingSymbol)
    {
        ArgumentNullException.ThrowIfNull(tradingSymbol);

        if (TradingSymbol == tradingSymbol)
        {
            return;
        }

        TradingSymbol = tradingSymbol;
    }

    /// <summary>
    /// EN: Changes the quote currency.
    /// FA: ارز مظنه Listing را تغییر می‌دهد.
    /// </summary>
    public void ChangeQuoteCurrency(CurrencyId quoteCurrencyId)
    {
        ArgumentNullException.ThrowIfNull(quoteCurrencyId);

        if (QuoteCurrencyId == quoteCurrencyId)
        {
            return;
        }

        QuoteCurrencyId = quoteCurrencyId;
    }

    /// <summary>
    /// EN: Changes tick size and price precision together.
    /// FA: TickSize و PricePrecision را به‌صورت هم‌زمان تغییر می‌دهد.
    /// </summary>
    public void ChangePricing(
        decimal tickSize,
        byte pricePrecision)
    {
        ValidatePricing(tickSize, pricePrecision);

        TickSize = tickSize;
        PricePrecision = pricePrecision;
    }

    /// <summary>
    /// EN: Marks this listing as the primary listing.
    /// FA: این Listing را به‌عنوان پذیرش اصلی تعیین می‌کند.
    /// </summary>
    public void MakePrimary()
    {
        if (IsPrimary)
        {
            return;
        }

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
        {
            return;
        }

        IsPrimary = false;
        Raise(new ListingPrimaryRemovedEvent(Id));
    }

    /// <summary>
    /// EN: Activates the listing.
    /// FA: Listing را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        ActivatedOn = DateTimeOffset.UtcNow;
        DeactivatedOn = null;

        Raise(new ListingActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the listing.
    /// FA: Listing را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedOn = DateTimeOffset.UtcNow;

        Raise(new ListingDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Rounds a price using the listing tick size and precision.
    /// FA: قیمت را بر اساس TickSize و PricePrecision مربوط به Listing گرد می‌کند.
    /// </summary>
    public decimal RoundPrice(decimal price)
    {
        decimal rounded =
            Math.Round(price / TickSize) * TickSize;

        return Math.Round(
            rounded,
            PricePrecision,
            MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Creates a listing with a generated identifier.
    /// FA: یک Listing با شناسه تولیدشده ایجاد می‌کند.
    /// </summary>
    public static Listing Create(
        InstrumentId instrumentId,
        VenueId venueId,
        CurrencyId quoteCurrencyId,
        TradingSymbol tradingSymbol,
        decimal tickSize,
        byte pricePrecision)
    {
        return new Listing(
            ListingId.New(),
            instrumentId,
            venueId,
            quoteCurrencyId,
            tradingSymbol,
            tickSize,
            pricePrecision);
    }

    private static void ValidatePricing(
        decimal tickSize,
        byte pricePrecision)
    {
        if (tickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tickSize),
                "Tick size must be greater than zero.");
        }

        if (pricePrecision > 28)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pricePrecision),
                "Price precision must be between 0 and 28.");
        }
    }
}

/// <summary>
/// EN: Domain event raised when a listing becomes primary.
/// FA: رویداد دامنه هنگام اصلی شدن Listing.
/// </summary>
public sealed record ListingMadePrimaryEvent(
    ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when the primary flag is removed.
/// FA: رویداد دامنه هنگام حذف وضعیت اصلی Listing.
/// </summary>
public sealed record ListingPrimaryRemovedEvent(
    ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a listing is activated.
/// FA: رویداد دامنه هنگام فعال شدن Listing.
/// </summary>
public sealed record ListingActivatedEvent(
    ListingId ListingId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a listing is deactivated.
/// FA: رویداد دامنه هنگام غیرفعال شدن Listing.
/// </summary>
public sealed record ListingDeactivatedEvent(
    ListingId ListingId) : DomainEvent;
