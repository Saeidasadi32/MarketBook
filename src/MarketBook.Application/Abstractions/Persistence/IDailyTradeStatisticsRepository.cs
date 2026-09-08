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
/// EN: Defines persistence operations for daily trade statistics.
/// FA: عملیات ماندگاری آمار معاملات روزانه را تعریف می‌کند.
/// </summary>
public interface IDailyTradeStatisticsRepository
{
    /// <summary>
    /// EN: Gets a record by identifier.
    /// FA: رکورد را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<DailyTradeStatistics?> GetByIdAsync(
        DailyTradeStatisticsId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks uniqueness for Listing and trading date.
    /// FA: یکتایی Listing و تاریخ معاملاتی را بررسی می‌کند.
    /// </summary>
    Task<bool> ExistsAsync(
        ListingId listingId,
        DateOnly tradingDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a daily trade-statistics record.
    /// FA: رکورد آمار معاملات روزانه را اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        DailyTradeStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a record as modified.
    /// FA: رکورد را به‌عنوان تغییرکرده علامت می‌زند.
    /// </summary>
    void Update(DailyTradeStatistics statistics);

    /// <summary>
    /// EN: Gets paged daily trade-statistics records.
    /// FA: رکوردهای آمار معاملات روزانه را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    Task<PagedResult<DailyTradeStatistics>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
