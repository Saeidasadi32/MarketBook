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
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Venue aggregate.
/// FA: عملیات مربوط به ذخیره‌سازی Aggregate مربوط به بستر معاملاتی را تعریف می‌کند.
/// </summary>
public interface IVenueRepository
{
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
    Task<Venue?> GetByIdAsync(
        VenueId id,
        CancellationToken cancellationToken = default);

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
    Task<Venue?> GetByCodeAsync(
        VenueCode code,
        CancellationToken cancellationToken = default);

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
    /// EN: True if a venue with the specified code exists; otherwise false.
    /// FA: اگر بستری با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    Task<bool> ExistsAsync(
        VenueCode code,
        CancellationToken cancellationToken = default);

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
    /// EN: True if another venue with the specified code exists; otherwise false.
    /// FA: اگر بستر دیگری با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    Task<bool> ExistsAsync(
        VenueCode code,
        VenueId excludingId,
        CancellationToken cancellationToken = default);

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
    /// <returns>
    /// EN: An asynchronous operation representing the add action.
    /// FA: عملیات غیرهمزمان مربوط به افزودن بستر معاملاتی.
    /// </returns>
    Task AddAsync(
        Venue venue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a venue as modified in the persistence context.
    /// FA: یک بستر معاملاتی را به‌عنوان تغییرکرده در Context ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="venue">
    /// EN: Venue aggregate to update.
    /// FA: Aggregate بستر معاملاتی برای به‌روزرسانی.
    /// </param>
    void Update(Venue venue);

    /// <summary>
    /// EN: Marks a venue for removal from persistence.
    /// FA: یک بستر معاملاتی را برای حذف از ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="venue">
    /// EN: Venue aggregate to remove.
    /// FA: Aggregate بستر معاملاتی برای حذف.
    /// </param>
    void Remove(Venue venue);

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
    Task<PagedResult<Venue>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
