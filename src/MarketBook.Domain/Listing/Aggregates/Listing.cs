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
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Domain.Listing.Aggregates;

/// <summary>
/// EN: Represents a tradable listing of an instrument on a market.
/// FA: نمایش‌دهنده پذیرش یک ابزار مالی در یک بازار مشخص است.
/// </summary>
public sealed class Listing : AggregateRoot
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Listing"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Listing"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Listing identifier.
    /// FA: شناسه پذیرش.
    /// </param>
    /// <param name="instrumentId">
    /// EN: Instrument identifier.
    /// FA: شناسه ابزار مالی.
    /// </param>
    /// <param name="marketId">
    /// EN: Market identifier.
    /// FA: شناسه بازار.
    /// </param>
    /// <param name="tradingSymbol">
    /// EN: Trading symbol.
    /// FA: نماد معاملاتی.
    /// </param>
    /// <param name="tickSize">
    /// EN: Minimum price increment.
    /// FA: حداقل گام تغییر قیمت.
    /// </param>
    /// <param name="pricePrecision">
    /// EN: Number of decimal digits allowed for prices.
    /// FA: تعداد ارقام اعشار مجاز برای قیمت.
    /// </param>
    public Listing(
        ListingId id,
        InstrumentId instrumentId,
        MarketId marketId,
        CurrencyId currencyId,
        TradingSymbol tradingSymbol,
        decimal tickSize,
        byte pricePrecision)
    {
        if (tickSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(tickSize));

        Id = id;
        InstrumentId = instrumentId;
        MarketId = marketId;
        TradingSymbol = tradingSymbol;
        TickSize = tickSize;
        PricePrecision = pricePrecision;
        CurrencyId = currencyId;

        IsPrimary = false;
        IsActive = true;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Gets the listing identifier.
    /// FA: شناسه پذیرش را دریافت می‌کند.
    /// </summary>
    public ListingId Id { get; }

    /// <summary>
    /// EN: Gets the related instrument identifier.
    /// FA: شناسه ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentId InstrumentId { get; }

    /// <summary>
    /// EN: Gets the market identifier.
    /// FA: شناسه بازار را دریافت می‌کند.
    /// </summary>
    public MarketId MarketId { get; }

    /// <summary>
    /// EN: Gets the trading symbol.
    /// FA: نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public TradingSymbol TradingSymbol { get; }

    public CurrencyId CurrencyId { get; }

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
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets whether the listing is active.
    /// FA: مشخص می‌کند پذیرش فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Marks this listing as the primary listing.
    /// FA: این پذیرش را به عنوان پذیرش اصلی تعیین می‌کند.
    /// </summary>
    public void MakePrimary()
    {
        IsPrimary = true;
    }

    /// <summary>
    /// EN: Removes the primary flag.
    /// FA: وضعیت پذیرش اصلی را حذف می‌کند.
    /// </summary>
    public void RemovePrimary()
    {
        IsPrimary = false;
    }

    /// <summary>
    /// EN: Activates the listing.
    /// FA: پذیرش را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// EN: Deactivates the listing.
    /// FA: پذیرش را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}