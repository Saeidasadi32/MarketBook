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
/// EN: Defines persistence operations for intraday price ticks.
/// FA: عملیات ماندگاری Tickهای قیمت درون‌روزی را تعریف می‌کند.
/// </summary>
public interface IIntradayPriceTickRepository
{
    /// <summary>
    /// EN: Gets a tick by identifier.
    /// FA: Tick را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<IntradayPriceTick?> GetByIdAsync(
        IntradayPriceTickId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks uniqueness of Listing, trading date, and sequence number.
    /// FA: یکتایی Listing، تاریخ معاملاتی و شماره توالی را بررسی می‌کند.
    /// </summary>
    Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        long sequenceNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds an immutable intraday tick.
    /// FA: یک Tick درون‌روزی تغییرناپذیر اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        IntradayPriceTick tick,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets ticks for one Listing and trading date.
    /// FA: Tickهای یک Listing و تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    Task<PagedResult<IntradayPriceTick>> GetPagedAsync(
        ListingId listingId,
        DateOnly tradingDate,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
