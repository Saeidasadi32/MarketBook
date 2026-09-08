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
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for trading calendars.
/// FA: عملیات ماندگاری تقویم‌های معاملاتی را تعریف می‌کند.
/// </summary>
public interface ITradingCalendarRepository
{
    /// <summary>
    /// EN: Gets a calendar by identifier including its sessions and date exceptions.
    /// FA: تقویم را همراه Sessionها و استثناهای تاریخی بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<TradingCalendar?> GetByIdAsync(
        TradingCalendarId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks whether a market already has a calendar for a year.
    /// FA: بررسی می‌کند آیا بازار برای سال مشخص تقویم دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        MarketId marketId,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a trading calendar.
    /// FA: تقویم معاملاتی را اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        TradingCalendar calendar,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a trading calendar as modified.
    /// FA: تقویم معاملاتی را تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    void Update(TradingCalendar calendar);

    /// <summary>
    /// EN: Gets paged trading calendars.
    /// FA: تقویم‌های معاملاتی را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    Task<PagedResult<TradingCalendar>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
