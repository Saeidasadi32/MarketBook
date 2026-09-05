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
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Venue.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Implements persistence operations for Listing.
/// FA: عملیات ذخیره‌سازی Listing را پیاده‌سازی می‌کند.
/// </summary>
public sealed class ListingRepository : IListingRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the repository.
    /// FA: Repository را مقداردهی می‌کند.
    /// </summary>
    public ListingRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Gets a listing by identifier.
    /// FA: Listing را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    public Task<Listing?> GetByIdAsync(
        ListingId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _dbContext.Set<Listing>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                listing => listing.Id == id,
                cancellationToken);
    }

    /// <summary>
    /// EN: Checks venue-scoped trading-symbol uniqueness.
    /// FA: یکتایی نماد معاملاتی را در محدوده Venue بررسی می‌کند.
    /// </summary>
    public Task<bool> ExistsAsync(
        VenueId venueId,
        TradingSymbol tradingSymbol,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(tradingSymbol);

        return _dbContext.Set<Listing>()
            .AnyAsync(
                listing =>
                    listing.VenueId == venueId &&
                    listing.TradingSymbol == tradingSymbol,
                cancellationToken);
    }

    /// <summary>
    /// EN: Checks venue-scoped trading-symbol uniqueness while excluding one listing.
    /// FA: یکتایی نماد معاملاتی را در Venue با مستثنا کردن یک Listing بررسی می‌کند.
    /// </summary>
    public Task<bool> ExistsAsync(
        VenueId venueId,
        TradingSymbol tradingSymbol,
        ListingId excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(tradingSymbol);
        ArgumentNullException.ThrowIfNull(excludingId);

        return _dbContext.Set<Listing>()
            .AnyAsync(
                listing =>
                    listing.VenueId == venueId &&
                    listing.TradingSymbol == tradingSymbol &&
                    listing.Id != excludingId,
                cancellationToken);
    }

    /// <summary>
    /// EN: Gets the current primary listing for an instrument.
    /// FA: Listing اصلی فعلی Instrument را دریافت می‌کند.
    /// </summary>
    public Task<Listing?> GetPrimaryByInstrumentIdAsync(
        InstrumentId instrumentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instrumentId);

        return _dbContext.Set<Listing>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                listing =>
                    listing.InstrumentId == instrumentId &&
                    listing.IsPrimary,
                cancellationToken);
    }

    /// <summary>
    /// EN: Adds a listing.
    /// FA: Listing را اضافه می‌کند.
    /// </summary>
    public async Task AddAsync(
        Listing listing,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(listing);

        await _dbContext.Set<Listing>()
            .AddAsync(listing, cancellationToken);
    }

    /// <summary>
    /// EN: Marks a listing as modified.
    /// FA: Listing را تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    public void Update(Listing listing)
    {
        ArgumentNullException.ThrowIfNull(listing);
        _dbContext.Set<Listing>().Update(listing);
    }

    /// <summary>
    /// EN: Gets a paged collection of listings ordered by trading symbol.
    /// FA: مجموعه صفحه‌بندی‌شده Listingها را بر اساس نماد معاملاتی دریافت می‌کند.
    /// </summary>
    public async Task<PagedResult<Listing>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Listing> query =
            _dbContext.Set<Listing>()
                .AsNoTracking()
                .OrderBy(listing => listing.TradingSymbol);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<Listing> items =
            await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Listing>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
