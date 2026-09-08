// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot;

/// <summary>EN: API request for one order-book level. FA: درخواست API برای یک سطح دفتر سفارشات.</summary>
public sealed record CreateOrderBookSnapshotLevelRequest(
    int Level,
    decimal? BidPrice,
    long? BidVolume,
    int? BidOrderCount,
    decimal? AskPrice,
    long? AskVolume,
    int? AskOrderCount);

/// <summary>EN: API request for creating an order-book snapshot. FA: درخواست API برای ایجاد Snapshot دفتر سفارشات.</summary>
public sealed record CreateOrderBookSnapshotRequest(
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset CapturedAt,
    long SequenceNumber,
    IReadOnlyList<CreateOrderBookSnapshotLevelRequest> Levels);
