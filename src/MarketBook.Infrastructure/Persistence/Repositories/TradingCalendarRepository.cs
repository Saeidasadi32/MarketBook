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
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Implements EF Core persistence for trading calendars.
/// FA: ماندگاری EF Core تقویم‌های معاملاتی را پیاده‌سازی می‌کند.
/// </summary>
public sealed class TradingCalendarRepository : ITradingCalendarRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the repository.
    /// FA: Repository را مقداردهی می‌کند.
    /// </summary>
    public TradingCalendarRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<TradingCalendar?> GetByIdAsync(
        TradingCalendarId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _dbContext.Set<TradingCalendar>()
            .Include(calendar => calendar.DateExceptions)
            .Include(calendar => calendar.Sessions)
            .SingleOrDefaultAsync(
                calendar => calendar.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        MarketId marketId,
        int year,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(marketId);

        return _dbContext.Set<TradingCalendar>()
            .AnyAsync(
                calendar =>
                    calendar.MarketId == marketId &&
                    calendar.Year == year,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        TradingCalendar calendar,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(calendar);

        await _dbContext.Set<TradingCalendar>()
            .AddAsync(calendar, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(TradingCalendar calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        _dbContext.Set<TradingCalendar>().Update(calendar);
    }

    /// <inheritdoc />
    public async Task<PagedResult<TradingCalendar>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<TradingCalendar> query =
            _dbContext.Set<TradingCalendar>()
                .AsNoTracking()
                .OrderByDescending(calendar => calendar.Year)
                .ThenBy(calendar => calendar.CreatedOn);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<TradingCalendar> items =
            await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<TradingCalendar>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
