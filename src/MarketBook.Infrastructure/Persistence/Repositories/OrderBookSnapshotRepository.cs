// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.MarketData.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>EN: Provides EF persistence for order-book snapshots. FA: ماندگاری EF برای Snapshotهای دفتر سفارشات را فراهم می‌کند.</summary>
public sealed class OrderBookSnapshotRepository
    : IOrderBookSnapshotRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the repository. FA: Repository را مقداردهی می‌کند.</summary>
    public OrderBookSnapshotRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<OrderBookSnapshot?> GetByIdAsync(
        OrderBookSnapshotId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<OrderBookSnapshot>()
            .AsNoTracking()
            .Include(item => item.Levels)
            .AsSplitQuery()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        long sequenceNumber,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<OrderBookSnapshot>()
            .AnyAsync(
                item => item.ListingId == listingId &&
                        item.TradingDate == tradingDate &&
                        item.SequenceNumber == sequenceNumber,
                cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(
        OrderBookSnapshot snapshot,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<OrderBookSnapshot>()
            .AddAsync(snapshot, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<OrderBookSnapshot>> GetPagedAsync(
        ListingId listingId,
        DateOnly tradingDate,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<OrderBookSnapshot> baseQuery =
            _dbContext.Set<OrderBookSnapshot>()
                .AsNoTracking()
                .Where(item =>
                    item.ListingId == listingId &&
                    item.TradingDate == tradingDate);

        int totalCount = await baseQuery.CountAsync(cancellationToken);

        List<OrderBookSnapshot> items = await baseQuery
            .OrderBy(item => item.CapturedAt)
            .ThenBy(item => item.SequenceNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(item => item.Levels)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderBookSnapshot>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
