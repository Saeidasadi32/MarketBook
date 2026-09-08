// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.MarketData.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for order-book snapshots.
/// FA: عملیات ماندگاری Snapshotهای دفتر سفارشات را تعریف می‌کند.
/// </summary>
public interface IOrderBookSnapshotRepository
{
    /// <summary>EN: Gets a snapshot by identifier. FA: Snapshot را بر اساس شناسه دریافت می‌کند.</summary>
    Task<OrderBookSnapshot?> GetByIdAsync(
        OrderBookSnapshotId id,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Checks source-sequence uniqueness. FA: یکتایی شماره توالی منبع را بررسی می‌کند.</summary>
    Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        long sequenceNumber,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Adds an immutable snapshot. FA: یک Snapshot تغییرناپذیر اضافه می‌کند.</summary>
    Task AddAsync(
        OrderBookSnapshot snapshot,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Gets snapshots for one Listing and trading date. FA: Snapshotهای یک Listing و تاریخ معاملاتی را دریافت می‌کند.</summary>
    Task<PagedResult<OrderBookSnapshot>> GetPagedAsync(
        ListingId listingId,
        DateOnly tradingDate,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
