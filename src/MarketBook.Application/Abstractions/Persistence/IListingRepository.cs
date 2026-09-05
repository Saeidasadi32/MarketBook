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
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Venue.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for Listing.
/// FA: عملیات ذخیره‌سازی Listing را تعریف می‌کند.
/// </summary>
public interface IListingRepository
{
    /// <summary>
    /// EN: Gets a listing by identifier.
    /// FA: Listing را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<Listing?> GetByIdAsync(
        ListingId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether a venue already uses the specified trading symbol.
    /// FA: بررسی می‌کند آیا Venue از نماد معاملاتی مشخص‌شده استفاده می‌کند یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        VenueId venueId,
        TradingSymbol tradingSymbol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether another listing in the venue uses the specified trading symbol.
    /// FA: بررسی می‌کند آیا Listing دیگری در Venue از نماد معاملاتی مشخص‌شده استفاده می‌کند یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        VenueId venueId,
        TradingSymbol tradingSymbol,
        ListingId excludingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the current primary listing for an instrument.
    /// FA: Listing اصلی فعلی یک Instrument را دریافت می‌کند.
    /// </summary>
    Task<Listing?> GetPrimaryByInstrumentIdAsync(
        InstrumentId instrumentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a listing.
    /// FA: Listing را اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        Listing listing,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a listing as modified.
    /// FA: Listing را تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    void Update(Listing listing);

    /// <summary>
    /// EN: Gets a paged collection of listings.
    /// FA: مجموعه صفحه‌بندی‌شده Listingها را دریافت می‌کند.
    /// </summary>
    Task<PagedResult<Listing>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
