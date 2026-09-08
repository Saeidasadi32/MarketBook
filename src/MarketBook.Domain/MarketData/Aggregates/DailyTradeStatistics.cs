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
/// EN: Represents official daily trading statistics for one Listing and trading date.
/// FA: آمار رسمی معاملات روزانه یک Listing در یک تاریخ معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed class DailyTradeStatistics : AggregateRoot<DailyTradeStatisticsId>
{
    /// <summary>
    /// EN: Initializes a daily trade-statistics record.
    /// FA: یک رکورد آمار معاملات روزانه را مقداردهی می‌کند.
    /// </summary>
    public DailyTradeStatistics(
        DailyTradeStatisticsId id,
        ListingId listingId,
        DateOnly tradingDate,
        long volume,
        int tradeCount,
        decimal averagePrice,
        decimal tradeValue,
        decimal marketCapitalization)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(listingId);

        Validate(
            volume,
            tradeCount,
            averagePrice,
            tradeValue,
            marketCapitalization);

        ListingId = listingId;
        TradingDate = tradingDate;
        Apply(
            volume,
            tradeCount,
            averagePrice,
            tradeValue,
            marketCapitalization);

        CreatedOn = DateTimeOffset.UtcNow;
        UpdatedOn = CreatedOn;
    }

    private DailyTradeStatistics()
    {
    }

    /// <summary>
    /// EN: Gets the Listing identifier.
    /// FA: شناسه Listing را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the trading date.
    /// FA: تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateOnly TradingDate { get; private set; }

    /// <summary>
    /// EN: Gets total traded volume.
    /// FA: حجم کل معاملات را دریافت می‌کند.
    /// </summary>
    public long Volume { get; private set; }

    /// <summary>
    /// EN: Gets the number of executed trades.
    /// FA: تعداد معاملات انجام‌شده را دریافت می‌کند.
    /// </summary>
    public int TradeCount { get; private set; }

    /// <summary>
    /// EN: Gets the average traded price.
    /// FA: میانگین قیمت معاملات را دریافت می‌کند.
    /// </summary>
    public decimal AveragePrice { get; private set; }

    /// <summary>
    /// EN: Gets total traded value.
    /// FA: ارزش کل معاملات را دریافت می‌کند.
    /// </summary>
    public decimal TradeValue { get; private set; }

    /// <summary>
    /// EN: Gets market capitalization for the trading date.
    /// FA: ارزش بازار در تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public decimal MarketCapitalization { get; private set; }

    /// <summary>
    /// EN: Gets creation time.
    /// FA: زمان ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets last update time.
    /// FA: زمان آخرین به‌روزرسانی را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset UpdatedOn { get; private set; }

    /// <summary>
    /// EN: Gets average traded volume per executed trade.
    /// FA: میانگین حجم معامله‌شده به ازای هر معامله را دریافت می‌کند.
    /// </summary>
    public decimal AverageTradeSize =>
        TradeCount == 0 ? 0m : (decimal)Volume / TradeCount;

    /// <summary>
    /// EN: Updates the mutable daily statistics.
    /// FA: آمار قابل تغییر روزانه را به‌روزرسانی می‌کند.
    /// </summary>
    public void Update(
        long volume,
        int tradeCount,
        decimal averagePrice,
        decimal tradeValue,
        decimal marketCapitalization)
    {
        Validate(
            volume,
            tradeCount,
            averagePrice,
            tradeValue,
            marketCapitalization);

        Apply(
            volume,
            tradeCount,
            averagePrice,
            tradeValue,
            marketCapitalization);

        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Creates a daily trade-statistics record.
    /// FA: یک رکورد آمار معاملات روزانه ایجاد می‌کند.
    /// </summary>
    public static DailyTradeStatistics Create(
        ListingId listingId,
        DateOnly tradingDate,
        long volume,
        int tradeCount,
        decimal averagePrice,
        decimal tradeValue,
        decimal marketCapitalization)
        => new(
            DailyTradeStatisticsId.New(),
            listingId,
            tradingDate,
            volume,
            tradeCount,
            averagePrice,
            tradeValue,
            marketCapitalization);

    private void Apply(
        long volume,
        int tradeCount,
        decimal averagePrice,
        decimal tradeValue,
        decimal marketCapitalization)
    {
        Volume = volume;
        TradeCount = tradeCount;
        AveragePrice = averagePrice;
        TradeValue = tradeValue;
        MarketCapitalization = marketCapitalization;
    }

    private static void Validate(
        long volume,
        int tradeCount,
        decimal averagePrice,
        decimal tradeValue,
        decimal marketCapitalization)
    {
        if (volume < 0)
            throw new ArgumentOutOfRangeException(nameof(volume), "Volume cannot be negative.");

        if (tradeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(tradeCount), "Trade count cannot be negative.");

        if (averagePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(averagePrice), "Average price cannot be negative.");

        if (tradeValue < 0)
            throw new ArgumentOutOfRangeException(nameof(tradeValue), "Trade value cannot be negative.");

        if (marketCapitalization < 0)
            throw new ArgumentOutOfRangeException(
                nameof(marketCapitalization),
                "Market capitalization cannot be negative.");
    }
}
