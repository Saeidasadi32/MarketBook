// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;

namespace MarketBook.Domain.MarketData.Entities;

/// <summary>
/// EN: Represents the official daily trading data of a listing.
/// FA: اطلاعات رسمی معاملات روزانه یک نماد را نمایش می‌دهد.
/// </summary>
public sealed class DailyPrice: IEquatable<DailyPrice>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="DailyPrice"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="DailyPrice"/> را ایجاد می‌کند.
    /// </summary>
    public DailyPrice(
        Price openPrice,
        Price highPrice,
        Price lowPrice,
        Price lastPrice,
        Price closePrice,
        Price yesterdayPrice,
        Price referencePrice,
        Volume volume,
        TradeCount tradeCount,
        Money tradeValue,
        Volume baseVolume,
        Money marketCapitalization,
        PriceLimit upperLimit,
        PriceLimit lowerLimit)
    {
        if (lowPrice > highPrice)
            throw new ArgumentException("Low price cannot be greater than high price.");

        if (openPrice.Value < 0 || highPrice.Value < 0 || lowPrice.Value < 0 ||
            lastPrice.Value < 0 || closePrice.Value < 0 || yesterdayPrice.Value < 0 ||
            referencePrice.Value < 0)
        {
            throw new ArgumentException("All prices must be non-negative.");
        }

        if (tradeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(tradeCount));
         
        OpenPrice = openPrice;
        HighPrice = highPrice;
        LowPrice = lowPrice;

        LastPrice = lastPrice;
        ClosePrice = closePrice;

        YesterdayPrice = yesterdayPrice;
        ReferencePrice = referencePrice;

        Volume = volume;
        TradeCount = tradeCount;
        TradeValue = tradeValue;

        BaseVolume = baseVolume;

        MarketCapitalization = marketCapitalization;

        UpperLimit = upperLimit;
        LowerLimit = lowerLimit;
    }

    /// <summary>
    /// EN: Opening price.
    /// FA: قیمت آغازین.
    /// </summary>
    public Price OpenPrice { get; }

    /// <summary>
    /// EN: Highest price.
    /// FA: بیشترین قیمت.
    /// </summary>
    public Price HighPrice { get; }

    /// <summary>
    /// EN: Lowest price.
    /// FA: کمترین قیمت.
    /// </summary>
    public Price LowPrice { get; }

    /// <summary>
    /// EN: Last traded price.
    /// FA: آخرین قیمت معامله.
    /// </summary>
    public Price LastPrice { get; }

    /// <summary>
    /// EN: Official closing price.
    /// FA: قیمت پایانی.
    /// </summary>
    public Price ClosePrice { get; }

    /// <summary>
    /// EN: Previous closing price.
    /// FA: قیمت پایانی روز قبل.
    /// </summary>
    public Price YesterdayPrice { get; }

    /// <summary>
    /// EN: Reference price.
    /// FA: قیمت مبنا.
    /// </summary>
    public Price ReferencePrice { get; }

    /// <summary>
    /// EN: Trading volume.
    /// FA: حجم معاملات.
    /// </summary>
    public Volume Volume { get; }

    /// <summary>
    /// EN: Number of executed trades.
    /// FA: تعداد معاملات.
    /// </summary>
    public TradeCount TradeCount { get; }

    /// <summary>
    /// EN: Total traded value.
    /// FA: ارزش معاملات.
    /// </summary>
    public Money TradeValue { get; }

    /// <summary>
    /// EN: Base volume.
    /// FA: حجم مبنا.
    /// </summary>
    public Volume BaseVolume { get; }

    /// <summary>
    /// EN: Market capitalization.
    /// FA: ارزش بازار.
    /// </summary>
    public Money MarketCapitalization { get; }

    /// <summary>
    /// EN: Daily upper price limit.
    /// FA: سقف مجاز قیمت.
    /// </summary>
    public PriceLimit UpperLimit { get; }

    /// <summary>
    /// EN: Daily lower price limit.
    /// FA: کف مجاز قیمت.
    /// </summary>
    public PriceLimit LowerLimit { get; }

    /// <summary>
    /// EN: Indicates whether the instrument is locked at the upper limit.
    /// FA: مشخص می‌کند نماد در صف خرید قفل شده است.
    /// </summary>
    public bool IsBuyQueue => LastPrice >= UpperLimit.Value;
    /// <summary>
    /// EN: Indicates whether the instrument is locked at the lower limit.
    /// FA: مشخص می‌کند نماد در صف فروش قفل شده است.
    /// </summary>
    public bool IsSellQueue => LastPrice <= LowerLimit.Value;

    /// <summary>
    /// EN: Calculates the daily price change.
    /// FA: تغییر قیمت روزانه را محاسبه می‌کند.
    /// </summary>
    public decimal Change => ClosePrice.Value - YesterdayPrice.Value;

    /// <summary>
    /// EN: Calculates the daily percentage change.
    /// FA: درصد تغییر روزانه را محاسبه می‌کند.
    /// </summary>
    public PercentageChange PercentageChange {
        get {
            if (YesterdayPrice.Value == 0)
                return PercentageChange.Zero;

            return PercentageChange.Calculate(YesterdayPrice.Value, ClosePrice.Value);
        }
    }

    /// <summary>
    /// EN: Calculates the daily range (high - low).
    /// FA: محدوده روزانه (بیشترین - کمترین) را محاسبه می‌کند.
    /// </summary>
    public Price DailyRange => new(HighPrice.Value - LowPrice.Value);

    /// <summary>
    /// EN: Determines whether the price is at the upper limit.
    /// FA: مشخص می‌کند قیمت در سقف مجاز است یا خیر.
    /// </summary>
    public bool IsAtUpperLimit => LastPrice.Value >= UpperLimit.Value.Value;

    /// <summary>
    /// EN: Determines whether the price is at the lower limit.
    /// FA: مشخص می‌کند قیمت در کف مجاز است یا خیر.
    /// </summary>
    public bool IsAtLowerLimit => LastPrice.Value <= LowerLimit.Value.Value;

    /// <summary>
    /// EN: Determines whether the specified daily price is equal to the current.
    /// FA: تعیین می‌کند که قیمت روزانه مشخص شده با قیمت جاری برابر است یا خیر.
    /// </summary>
    public bool Equals(DailyPrice? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return OpenPrice == other.OpenPrice &&
               HighPrice == other.HighPrice &&
               LowPrice == other.LowPrice &&
               LastPrice == other.LastPrice &&
               ClosePrice == other.ClosePrice &&
               YesterdayPrice == other.YesterdayPrice &&
               ReferencePrice == other.ReferencePrice &&
               Volume == other.Volume &&
               TradeCount == other.TradeCount &&
               TradeValue == other.TradeValue &&
               BaseVolume == other.BaseVolume &&
               MarketCapitalization == other.MarketCapitalization &&
               UpperLimit == other.UpperLimit &&
               LowerLimit == other.LowerLimit;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as DailyPrice);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash1 = HashCode.Combine(
            OpenPrice, HighPrice, LowPrice, LastPrice,
            ClosePrice, YesterdayPrice, ReferencePrice, Volume);

        var hash2 = HashCode.Combine(
            TradeCount, TradeValue, BaseVolume, MarketCapitalization,
            UpperLimit, LowerLimit);

        return HashCode.Combine(hash1, hash2);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"DailyPrice [Open: {OpenPrice}, High: {HighPrice}, Low: {LowPrice}, Close: {ClosePrice}, Volume: {Volume}]";
}