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

/// <summary>
/// EN: Provides EF persistence for intraday price ticks.
/// FA: ماندگاری EF برای Tickهای قیمت درون‌روزی را فراهم می‌کند.
/// </summary>
public sealed class IntradayPriceTickRepository
    : IIntradayPriceTickRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the repository.
    /// FA: Repository را مقداردهی می‌کند.
    /// </summary>
    public IntradayPriceTickRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<IntradayPriceTick?> GetByIdAsync(
        IntradayPriceTickId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<IntradayPriceTick>()
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        long sequenceNumber,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<IntradayPriceTick>()
            .AnyAsync(
                item => item.ListingId == listingId &&
                        item.TradingDate == tradingDate &&
                        item.SequenceNumber == sequenceNumber,
                cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(
        IntradayPriceTick tick,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<IntradayPriceTick>()
            .AddAsync(tick, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<IntradayPriceTick>> GetPagedAsync(
        ListingId listingId,
        DateOnly tradingDate,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<IntradayPriceTick> query =
            _dbContext.Set<IntradayPriceTick>()
                .AsNoTracking()
                .Where(item =>
                    item.ListingId == listingId &&
                    item.TradingDate == tradingDate)
                .OrderBy(item => item.OccurredAt)
                .ThenBy(item => item.SequenceNumber);

        int totalCount = await query.CountAsync(cancellationToken);

        List<IntradayPriceTick> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<IntradayPriceTick>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
