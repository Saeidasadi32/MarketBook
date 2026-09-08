// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots;

/// <summary>EN: Query for paged order-book snapshots. FA: پرس‌وجوی صفحه‌بندی Snapshotهای دفتر سفارشات.</summary>
public sealed record GetOrderBookSnapshotsQuery(
    string ListingId,
    DateOnly TradingDate,
    int Page,
    int PageSize)
    : IRequest<Result<GetOrderBookSnapshotsResponse>>;
