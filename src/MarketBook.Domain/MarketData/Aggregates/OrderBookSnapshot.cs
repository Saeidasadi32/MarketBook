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
using MarketBook.Domain.MarketData.Entities;
using MarketBook.Domain.MarketData.ValueObjects;

namespace MarketBook.Domain.MarketData.Aggregates;

/// <summary>
/// EN: Represents one immutable order-book depth snapshot for a Listing.
/// FA: یک Snapshot تغییرناپذیر از عمق دفتر سفارشات یک Listing را نمایش می‌دهد.
/// </summary>
public sealed class OrderBookSnapshot : AggregateRoot<OrderBookSnapshotId>
{
    private readonly List<OrderBookSnapshotLevel> _levels = [];

    /// <summary>
    /// EN: Initializes an immutable order-book snapshot.
    /// FA: یک Snapshot تغییرناپذیر از دفتر سفارشات را مقداردهی می‌کند.
    /// </summary>
    public OrderBookSnapshot(
        OrderBookSnapshotId id,
        ListingId listingId,
        DateOnly tradingDate,
        DateTimeOffset capturedAt,
        long sequenceNumber,
        IEnumerable<OrderBookSnapshotLevel> levels)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(listingId);
        ArgumentNullException.ThrowIfNull(levels);

        if (sequenceNumber < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sequenceNumber),
                "Sequence number cannot be negative.");

        List<OrderBookSnapshotLevel> normalizedLevels = levels
            .OrderBy(item => item.Level)
            .ToList();

        ValidateLevels(normalizedLevels);

        ListingId = listingId;
        TradingDate = tradingDate;
        CapturedAt = capturedAt;
        SequenceNumber = sequenceNumber;
        CreatedOn = DateTimeOffset.UtcNow;
        _levels.AddRange(normalizedLevels);
    }

    private OrderBookSnapshot()
    {
    }

    /// <summary>EN: Gets the Listing identifier. FA: شناسه Listing را دریافت می‌کند.</summary>
    public ListingId ListingId { get; private set; } = default!;

    /// <summary>EN: Gets the logical trading date. FA: تاریخ معاملاتی منطقی را دریافت می‌کند.</summary>
    public DateOnly TradingDate { get; private set; }

    /// <summary>EN: Gets the source capture timestamp. FA: زمان ثبت Snapshot در منبع را دریافت می‌کند.</summary>
    public DateTimeOffset CapturedAt { get; private set; }

    /// <summary>EN: Gets the source sequence number. FA: شماره توالی Snapshot در منبع را دریافت می‌کند.</summary>
    public long SequenceNumber { get; private set; }

    /// <summary>EN: Gets persistence creation time. FA: زمان ایجاد رکورد را دریافت می‌کند.</summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>EN: Gets ordered depth levels. FA: سطوح مرتب‌شده عمق بازار را دریافت می‌کند.</summary>
    public IReadOnlyList<OrderBookSnapshotLevel> Levels => _levels;

    /// <summary>EN: Gets level one, which contains the best available bid/ask. FA: سطح اول شامل بهترین خرید/فروش موجود را دریافت می‌کند.</summary>
    public OrderBookSnapshotLevel BestLevel => _levels[0];

    /// <summary>
    /// EN: Creates one immutable order-book snapshot.
    /// FA: یک Snapshot تغییرناپذیر از دفتر سفارشات ایجاد می‌کند.
    /// </summary>
    public static OrderBookSnapshot Create(
        ListingId listingId,
        DateOnly tradingDate,
        DateTimeOffset capturedAt,
        long sequenceNumber,
        IEnumerable<OrderBookSnapshotLevel> levels)
        => new(
            OrderBookSnapshotId.New(),
            listingId,
            tradingDate,
            capturedAt,
            sequenceNumber,
            levels);

    private static void ValidateLevels(
        IReadOnlyList<OrderBookSnapshotLevel> levels)
    {
        if (levels.Count == 0)
            throw new ArgumentException(
                "An order-book snapshot must contain at least one level.",
                nameof(levels));

        for (int index = 0; index < levels.Count; index++)
        {
            int expectedLevel = index + 1;

            if (levels[index].Level != expectedLevel)
                throw new ArgumentException(
                    "Order-book levels must be unique and contiguous starting at level 1.",
                    nameof(levels));
        }

        decimal? previousBid = null;
        decimal? previousAsk = null;

        foreach (OrderBookSnapshotLevel level in levels)
        {
            if (level.BidPrice.HasValue)
            {
                if (previousBid.HasValue && level.BidPrice.Value > previousBid.Value)
                    throw new ArgumentException(
                        "Bid prices must not increase as depth level increases.",
                        nameof(levels));

                previousBid = level.BidPrice.Value;
            }

            if (level.AskPrice.HasValue)
            {
                if (previousAsk.HasValue && level.AskPrice.Value < previousAsk.Value)
                    throw new ArgumentException(
                        "Ask prices must not decrease as depth level increases.",
                        nameof(levels));

                previousAsk = level.AskPrice.Value;
            }
        }
    }
}
