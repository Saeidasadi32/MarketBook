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
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Implements persistence operations for Instrument.
/// FA: عملیات ذخیره‌سازی Instrument را پیاده‌سازی می‌کند.
/// </summary>
public sealed class InstrumentRepository : IInstrumentRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new repository.
    /// FA: Repository جدید را مقداردهی می‌کند.
    /// </summary>
    public InstrumentRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Gets an instrument by identifier.
    /// FA: ابزار مالی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    public Task<Instrument?> GetByIdAsync(
        InstrumentId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _dbContext.Set<Instrument>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                instrument => instrument.Id == id,
                cancellationToken);
    }

    /// <summary>
    /// EN: Gets an instrument by ISIN.
    /// FA: ابزار مالی را بر اساس ISIN دریافت می‌کند.
    /// </summary>
    public Task<Instrument?> GetByIsinAsync(
        Isin isin,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(isin);

        return _dbContext.Set<Instrument>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                instrument => instrument.Isin == isin,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether the ISIN already exists.
    /// FA: وجود ISIN را بررسی می‌کند.
    /// </summary>
    public Task<bool> ExistsAsync(
        Isin isin,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(isin);

        return _dbContext.Set<Instrument>()
            .AnyAsync(
                instrument => instrument.Isin == isin,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether another instrument uses the ISIN.
    /// FA: بررسی می‌کند آیا ابزار دیگری از ISIN استفاده می‌کند یا خیر.
    /// </summary>
    public Task<bool> ExistsAsync(
        Isin isin,
        InstrumentId excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(isin);
        ArgumentNullException.ThrowIfNull(excludingId);

        return _dbContext.Set<Instrument>()
            .AnyAsync(
                instrument =>
                    instrument.Isin == isin &&
                    instrument.Id != excludingId,
                cancellationToken);
    }

    /// <summary>
    /// EN: Adds a new instrument.
    /// FA: ابزار مالی جدید را اضافه می‌کند.
    /// </summary>
    public async Task AddAsync(
        Instrument instrument,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instrument);

        await _dbContext.Set<Instrument>()
            .AddAsync(instrument, cancellationToken);
    }

    /// <summary>
    /// EN: Marks an instrument as modified.
    /// FA: ابزار مالی را تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    public void Update(Instrument instrument)
    {
        ArgumentNullException.ThrowIfNull(instrument);
        _dbContext.Set<Instrument>().Update(instrument);
    }

    /// <summary>
    /// EN: Gets a paged collection of instruments.
    /// FA: مجموعه صفحه‌بندی‌شده ابزارهای مالی را دریافت می‌کند.
    /// </summary>
    public async Task<PagedResult<Instrument>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Instrument> query =
            _dbContext.Set<Instrument>()
                .AsNoTracking()
                .OrderBy(instrument => instrument.Name);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<Instrument> items =
            await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Instrument>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
