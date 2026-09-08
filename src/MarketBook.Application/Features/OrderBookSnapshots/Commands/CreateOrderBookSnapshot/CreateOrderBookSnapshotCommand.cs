// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot;

/// <summary>EN: Command for creating an order-book snapshot. FA: فرمان ایجاد Snapshot دفتر سفارشات.</summary>
public sealed record CreateOrderBookSnapshotCommand(
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset CapturedAt,
    long SequenceNumber,
    IReadOnlyList<CreateOrderBookSnapshotLevelRequest> Levels)
    : IRequest<Result<OrderBookSnapshotId>>;
