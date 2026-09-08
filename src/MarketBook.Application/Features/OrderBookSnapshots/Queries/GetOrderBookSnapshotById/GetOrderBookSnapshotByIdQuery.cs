// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById;

/// <summary>EN: Query for one order-book snapshot. FA: پرس‌وجوی دریافت یک Snapshot دفتر سفارشات.</summary>
public sealed record GetOrderBookSnapshotByIdQuery(string Id)
    : IRequest<Result<GetOrderBookSnapshotByIdResponse>>;
