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
/// <summary>EN: Represents official daily market prices and permitted price limits for a Listing. FA: قیمت‌های رسمی روزانه و دامنه مجاز قیمت یک Listing را نمایش می‌دهد.</summary>
public sealed class MarketPrice : AggregateRoot<MarketPriceId>
{
    public MarketPrice(MarketPriceId id, ListingId listingId, DateOnly tradingDate, decimal openPrice, decimal highPrice, decimal lowPrice, decimal lastPrice, decimal closePrice, decimal previousClosePrice, decimal referencePrice, decimal lowerLimit, decimal upperLimit) : base(id)
    {
        ArgumentNullException.ThrowIfNull(listingId);
        Validate(openPrice, highPrice, lowPrice, lastPrice, closePrice, previousClosePrice, referencePrice, lowerLimit, upperLimit);
        ListingId=listingId; TradingDate=tradingDate; Apply(openPrice,highPrice,lowPrice,lastPrice,closePrice,previousClosePrice,referencePrice,lowerLimit,upperLimit); CreatedOn=DateTimeOffset.UtcNow; UpdatedOn=CreatedOn;
    }
    private MarketPrice() { }
/// <summary>EN: Gets the Listing identifier. FA: شناسه Listing را دریافت می‌کند.</summary>
public ListingId ListingId { get; private set; } = default!;
/// <summary>EN: Gets the trading date. FA: تاریخ معاملاتی را دریافت می‌کند.</summary>
public DateOnly TradingDate { get; private set; }
/// <summary>EN: Gets opening price. FA: قیمت آغازین را دریافت می‌کند.</summary>
public decimal OpenPrice { get; private set; }
/// <summary>EN: Gets highest price. FA: بیشترین قیمت را دریافت می‌کند.</summary>
public decimal HighPrice { get; private set; }
/// <summary>EN: Gets lowest price. FA: کمترین قیمت را دریافت می‌کند.</summary>
public decimal LowPrice { get; private set; }
/// <summary>EN: Gets last traded price. FA: آخرین قیمت معامله را دریافت می‌کند.</summary>
public decimal LastPrice { get; private set; }
/// <summary>EN: Gets official close price. FA: قیمت پایانی را دریافت می‌کند.</summary>
public decimal ClosePrice { get; private set; }
/// <summary>EN: Gets previous close price. FA: قیمت پایانی روز قبل را دریافت می‌کند.</summary>
public decimal PreviousClosePrice { get; private set; }
/// <summary>EN: Gets reference price. FA: قیمت مبنا را دریافت می‌کند.</summary>
public decimal ReferencePrice { get; private set; }
/// <summary>EN: Gets lower permitted price. FA: کف مجاز قیمت را دریافت می‌کند.</summary>
public decimal LowerLimit { get; private set; }
/// <summary>EN: Gets upper permitted price. FA: سقف مجاز قیمت را دریافت می‌کند.</summary>
public decimal UpperLimit { get; private set; }
/// <summary>EN: Gets creation time. FA: زمان ایجاد را دریافت می‌کند.</summary>
public DateTimeOffset CreatedOn { get; private set; }
/// <summary>EN: Gets last update time. FA: زمان آخرین به‌روزرسانی را دریافت می‌کند.</summary>
public DateTimeOffset UpdatedOn { get; private set; }
/// <summary>EN: Indicates last price is at upper limit. FA: مشخص می‌کند آخرین قیمت در سقف مجاز است.</summary>
public bool IsAtUpperLimit => LastPrice >= UpperLimit;
/// <summary>EN: Indicates last price is at lower limit. FA: مشخص می‌کند آخرین قیمت در کف مجاز است.</summary>
public bool IsAtLowerLimit => LastPrice <= LowerLimit;
/// <summary>EN: Updates daily prices and limits. FA: قیمت‌ها و دامنه مجاز روز را به‌روزرسانی می‌کند.</summary>
public void Update(decimal openPrice, decimal highPrice, decimal lowPrice, decimal lastPrice, decimal closePrice, decimal previousClosePrice, decimal referencePrice, decimal lowerLimit, decimal upperLimit)
    { Validate(openPrice,highPrice,lowPrice,lastPrice,closePrice,previousClosePrice,referencePrice,lowerLimit,upperLimit); Apply(openPrice,highPrice,lowPrice,lastPrice,closePrice,previousClosePrice,referencePrice,lowerLimit,upperLimit); UpdatedOn=DateTimeOffset.UtcNow; }
/// <summary>EN: Creates a market-price record. FA: رکورد قیمت بازار ایجاد می‌کند.</summary>
public static MarketPrice Create(ListingId listingId, DateOnly tradingDate, decimal openPrice, decimal highPrice, decimal lowPrice, decimal lastPrice, decimal closePrice, decimal previousClosePrice, decimal referencePrice, decimal lowerLimit, decimal upperLimit) => new(MarketPriceId.New(),listingId,tradingDate,openPrice,highPrice,lowPrice,lastPrice,closePrice,previousClosePrice,referencePrice,lowerLimit,upperLimit);
    private void Apply(decimal openPrice,decimal highPrice,decimal lowPrice,decimal lastPrice,decimal closePrice,decimal previousClosePrice,decimal referencePrice,decimal lowerLimit,decimal upperLimit) { OpenPrice=openPrice; HighPrice=highPrice; LowPrice=lowPrice; LastPrice=lastPrice; ClosePrice=closePrice; PreviousClosePrice=previousClosePrice; ReferencePrice=referencePrice; LowerLimit=lowerLimit; UpperLimit=upperLimit; }
    private static void Validate(decimal openPrice,decimal highPrice,decimal lowPrice,decimal lastPrice,decimal closePrice,decimal previousClosePrice,decimal referencePrice,decimal lowerLimit,decimal upperLimit)
    {
        if (openPrice<0 || highPrice<0 || lowPrice<0 || lastPrice<0 || closePrice<0 || previousClosePrice<0 || referencePrice<0 || lowerLimit<0 || upperLimit<0) throw new ArgumentOutOfRangeException(nameof(openPrice),"Prices cannot be negative.");
        if (lowPrice>highPrice) throw new ArgumentException("Low price cannot be greater than high price.");
        if (lowerLimit>upperLimit) throw new ArgumentException("Lower limit cannot be greater than upper limit.");
        if (openPrice<lowerLimit || openPrice>upperLimit || highPrice<lowerLimit || highPrice>upperLimit || lowPrice<lowerLimit || lowPrice>upperLimit || lastPrice<lowerLimit || lastPrice>upperLimit || closePrice<lowerLimit || closePrice>upperLimit) throw new ArgumentException("Trading prices must be within the permitted price limits.");
    }
}
