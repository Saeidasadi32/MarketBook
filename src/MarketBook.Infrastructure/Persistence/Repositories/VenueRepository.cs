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
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Implements persistence operations for the Venue aggregate.
/// FA: عملیات ذخیره‌سازی Aggregate مربوط به بستر معاملاتی را پیاده‌سازی می‌کند.
/// </summary>
public sealed class VenueRepository : IVenueRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="VenueRepository"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="VenueRepository"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public VenueRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Gets a venue by its identifier.
    /// FA: یک بستر معاملاتی را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Venue identifier.
    /// FA: شناسه بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The venue if found; otherwise null.
    /// FA: در صورت یافتن، بستر معاملاتی و در غیر این صورت null.
    /// </returns>
    public async Task<Venue?> GetByIdAsync(
        VenueId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _dbContext.Set<Venue>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                venue => venue.Id == id,
                cancellationToken);
    }

    /// <summary>
    /// EN: Gets a venue by its business code.
    /// FA: یک بستر معاملاتی را بر اساس کد تجاری آن دریافت می‌کند.
    /// </summary>
    /// <param name="code">
    /// EN: Venue business code.
    /// FA: کد تجاری بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The venue if found; otherwise null.
    /// FA: در صورت یافتن، بستر معاملاتی و در غیر این صورت null.
    /// </returns>
    public async Task<Venue?> GetByCodeAsync(
        VenueCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Venue>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                venue => venue.Code == code,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether a venue with the specified code exists.
    /// FA: بررسی می‌کند آیا بستری با کد مشخص‌شده وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Venue business code.
    /// FA: کد تجاری بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if a matching venue exists; otherwise false.
    /// FA: اگر بستر معاملاتی مطابق وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    public async Task<bool> ExistsAsync(
        VenueCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Venue>()
            .AnyAsync(
                venue => venue.Code == code,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether another venue with the specified code exists,
    /// excluding the specified venue.
    /// FA: بررسی می‌کند آیا بستر معاملاتی دیگری با کد مشخص‌شده،
    /// به‌جز بستر تعیین‌شده، وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Venue business code.
    /// FA: کد تجاری بستر معاملاتی.
    /// </param>
    /// <param name="excludingId">
    /// EN: Venue identifier to exclude from the search.
    /// FA: شناسه بستری که باید از جستجو مستثنی شود.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if another matching venue exists; otherwise false.
    /// FA: اگر بستر دیگری با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    public async Task<bool> ExistsAsync(
        VenueCode code,
        VenueId excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(excludingId);

        return await _dbContext.Set<Venue>()
            .AnyAsync(
                venue =>
                    venue.Code == code &&
                    venue.Id != excludingId,
                cancellationToken);
    }

    /// <summary>
    /// EN: Adds a new venue to the persistence context.
    /// FA: یک بستر معاملاتی جدید را به Context ذخیره‌سازی اضافه می‌کند.
    /// </summary>
    /// <param name="venue">
    /// EN: Venue aggregate to add.
    /// FA: Aggregate بستر معاملاتی برای افزودن.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    public async Task AddAsync(
        Venue venue,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(venue);

        await _dbContext.Set<Venue>()
            .AddAsync(venue, cancellationToken);
    }

    /// <summary>
    /// EN: Marks a venue as modified in the persistence context.
    /// FA: یک بستر معاملاتی را به‌عنوان تغییرکرده در Context ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="venue">
    /// EN: Venue aggregate to update.
    /// FA: Aggregate بستر معاملاتی برای به‌روزرسانی.
    /// </param>
    public void Update(Venue venue)
    {
        ArgumentNullException.ThrowIfNull(venue);

        _dbContext.Set<Venue>().Update(venue);
    }

    /// <summary>
    /// EN: Marks a venue for removal from persistence.
    /// FA: یک بستر معاملاتی را برای حذف از ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="venue">
    /// EN: Venue aggregate to remove.
    /// FA: Aggregate بستر معاملاتی برای حذف.
    /// </param>
    public void Remove(Venue venue)
    {
        ArgumentNullException.ThrowIfNull(venue);

        _dbContext.Set<Venue>().Remove(venue);
    }

    /// <summary>
    /// EN: Gets a paged collection of venues.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از بسترهای معاملاتی را دریافت می‌کند.
    /// </summary>
    /// <param name="pageRequest">
    /// EN: Pagination parameters.
    /// FA: پارامترهای صفحه‌بندی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paged result containing the requested venues.
    /// FA: نتیجه صفحه‌بندی‌شده شامل بسترهای معاملاتی مورد درخواست.
    /// </returns>
    public async Task<PagedResult<Venue>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Venue> query = _dbContext.Set<Venue>()
            .AsNoTracking()
            .OrderBy(venue => venue.Code);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<Venue> items =
            await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Venue>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
