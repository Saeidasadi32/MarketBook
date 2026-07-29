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

namespace MarketBook.Domain.MarketData.Entities;

/// <summary>
/// EN: Represents daily trading statistics.
/// FA: آمار معاملات روزانه را نمایش می‌دهد.
/// </summary>
public sealed class TradeStatistics
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradeStatistics"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TradeStatistics"/> را ایجاد می‌کند.
    /// </summary>
    public TradeStatistics(
        Volume volume,
        int tradeCount,
        Price averagePrice,
        Price tradeValue,
        Price marketCapitalization)
    {
        if (tradeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(tradeCount));

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
    /// EN: Gets total traded value.
    /// FA: ارزش معاملات را دریافت می‌کند.
    /// </summary>
    public Price TradeValue { get; }

    /// <summary>
    /// EN: Gets market capitalization.
    /// FA: ارزش بازار را دریافت می‌کند.
    /// </summary>
    public Price MarketCapitalization { get; }
}