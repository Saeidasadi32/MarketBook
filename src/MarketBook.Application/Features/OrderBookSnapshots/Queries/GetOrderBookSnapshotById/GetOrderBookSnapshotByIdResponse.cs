// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById;

/// <summary>EN: Response level for one order-book snapshot. FA: سطح پاسخ مربوط به یک Snapshot دفتر سفارشات.</summary>
public sealed record OrderBookSnapshotLevelResponse(
    int Level,
    decimal? BidPrice,
    long? BidVolume,
    int? BidOrderCount,
    decimal? AskPrice,
    long? AskVolume,
    int? AskOrderCount);

/// <summary>EN: Response for one order-book snapshot. FA: پاسخ مربوط به یک Snapshot دفتر سفارشات.</summary>
public sealed record GetOrderBookSnapshotByIdResponse(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset CapturedAt,
    long SequenceNumber,
    IReadOnlyList<OrderBookSnapshotLevelResponse> Levels,
    DateTimeOffset CreatedOn);
