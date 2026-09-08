// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots;

/// <summary>EN: One order-book level in a paged response. FA: یک سطح دفتر سفارشات در پاسخ صفحه‌بندی‌شده.</summary>
public sealed record OrderBookSnapshotItemLevel(
    int Level,
    decimal? BidPrice,
    long? BidVolume,
    int? BidOrderCount,
    decimal? AskPrice,
    long? AskVolume,
    int? AskOrderCount);

/// <summary>EN: One order-book snapshot in a paged response. FA: یک Snapshot دفتر سفارشات در پاسخ صفحه‌بندی‌شده.</summary>
public sealed record OrderBookSnapshotItem(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset CapturedAt,
    long SequenceNumber,
    IReadOnlyList<OrderBookSnapshotItemLevel> Levels);

/// <summary>EN: Paged order-book snapshot response. FA: پاسخ صفحه‌بندی Snapshotهای دفتر سفارشات.</summary>
public sealed record GetOrderBookSnapshotsResponse(
    IReadOnlyList<OrderBookSnapshotItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
