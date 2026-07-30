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
/// EN: Represents daily trading statistics.
/// FA: آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed class TradeStatistics : IEquatable<TradeStatistics>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradeStatistics"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TradeStatistics"/> را ایجاد می‌کند.
    /// </summary>
    public TradeStatistics(
        Volume volume,
        int tradeCount,
        Price averagePrice,
        Money tradeValue,
        Money marketCapitalization)
    {
        if (tradeCount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(tradeCount),
                "Trade count cannot be negative.");

        if (averagePrice.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(averagePrice),
                "Average price cannot be negative.");

        Volume = volume;
        TradeCount = tradeCount;
        AveragePrice = averagePrice;
        TradeValue = tradeValue;
        MarketCapitalization = marketCapitalization;
    }

    /// <summary>
    /// EN: Gets traded volume.
    /// FA: حجم معاملات را دریافت می‌کند.
    /// </summary>
    public Volume Volume { get; }

    /// <summary>
    /// EN: Gets number of executed trades.
    /// FA: تعداد معاملات را دریافت می‌کند.
    /// </summary>
    public int TradeCount { get; }

    /// <summary>
    /// EN: Gets average traded price.
    /// FA: میانگین قیمت معاملات را دریافت می‌کند.
    /// </summary>
    public Price AveragePrice { get; }

    /// <summary>
    /// EN: Gets total traded value (in currency).
    /// FA: ارزش معاملات (به واحد پولی) را دریافت می‌کند.
    /// </summary>
    public Money TradeValue { get; }

    /// <summary>
    /// EN: Gets market capitalization (in currency).
    /// FA: ارزش بازار (به واحد پولی) را دریافت می‌کند.
    /// </summary>
    public Money MarketCapitalization { get; }

    /// <summary>
    /// EN: Calculates the average trade size.
    /// FA: متوسط حجم هر معامله را محاسبه می‌کند.
    /// </summary>
    public decimal AverageTradeSize()
    {
        if (TradeCount == 0)
            return 0;

        return Volume.Value / (decimal)TradeCount;
    }

    /// <summary>
    /// EN: Determines whether the specified statistics is equal to the current statistics.
    /// FA: تعیین می‌کند که آمار مشخص شده با آمار جاری برابر است یا خیر.
    /// </summary>
    public bool Equals(TradeStatistics? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Volume == other.Volume &&
               TradeCount == other.TradeCount &&
               AveragePrice == other.AveragePrice &&
               TradeValue == other.TradeValue &&
               MarketCapitalization == other.MarketCapitalization;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as TradeStatistics);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Volume, TradeCount, AveragePrice, TradeValue, MarketCapitalization);

    /// <summary>
    /// EN: Returns a string representation of the statistics.
    /// FA: نمایش رشته‌ای از آمار را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"Trades: {TradeCount}, Volume: {Volume}, Avg Price: {AveragePrice}, Value: {TradeValue}";
}