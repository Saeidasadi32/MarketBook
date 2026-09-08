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
/// EN: Provides EF persistence for daily trade statistics.
/// FA: ماندگاری EF برای آمار معاملات روزانه را فراهم می‌کند.
/// </summary>
public sealed class DailyTradeStatisticsRepository
    : IDailyTradeStatisticsRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the repository.
    /// FA: Repository را مقداردهی می‌کند.
    /// </summary>
    public DailyTradeStatisticsRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<DailyTradeStatistics?> GetByIdAsync(
        DailyTradeStatisticsId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<DailyTradeStatistics>()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<DailyTradeStatistics>()
            .AnyAsync(
                item => item.ListingId == listingId &&
                        item.TradingDate == tradingDate,
                cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(
        DailyTradeStatistics statistics,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<DailyTradeStatistics>()
            .AddAsync(statistics, cancellationToken);

    /// <inheritdoc />
    public void Update(DailyTradeStatistics statistics)
        => _dbContext.Set<DailyTradeStatistics>().Update(statistics);

    /// <inheritdoc />
    public async Task<PagedResult<DailyTradeStatistics>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<DailyTradeStatistics> query =
            _dbContext.Set<DailyTradeStatistics>()
                .AsNoTracking()
                .OrderByDescending(item => item.TradingDate)
                .ThenByDescending(item => item.CreatedOn);

        int totalCount = await query.CountAsync(cancellationToken);

        List<DailyTradeStatistics> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<DailyTradeStatistics>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
