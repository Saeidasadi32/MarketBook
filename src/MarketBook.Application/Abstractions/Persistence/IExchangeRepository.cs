// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Exchange aggregate.
/// FA: عملیات ماندگاری Aggregate بورس را تعریف می‌کند.
/// </summary>
public interface IExchangeRepository
{
    /// <summary>
    /// EN: Gets an exchange by its identifier.
    /// FA: بورس را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<Exchange?> GetByIdAsync(
        ExchangeId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets an exchange by its code.
    /// FA: بورس را بر اساس کد دریافت می‌کند.
    /// </summary>
    Task<Exchange?> GetByCodeAsync(
        ExchangeCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks whether an exchange code already exists.
    /// FA: بررسی می‌کند آیا کد بورس از قبل وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        ExchangeCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks whether an exchange code already exists
    /// for an exchange other than the specified exchange.
    /// FA: بررسی می‌کند آیا کد بورس برای بورسی غیر از بورس مشخص‌شده
    /// از قبل وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Exchange business code.
    /// FA: کد تجاری بورس.
    /// </param>
    /// <param name="excludingId">
    /// EN: Exchange identifier to exclude from the duplicate check.
    /// FA: شناسه بورسی که باید از بررسی تکراری بودن مستثنا شود.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    Task<bool> ExistsAsync(
        ExchangeCode code,
        ExchangeId excludingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds an exchange.
    /// FA: یک بورس اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Updates an existing exchange.
    /// FA: یک بورس موجود را به‌روزرسانی می‌کند.
    /// </summary>
    /// <param name="exchange"></param>
    void Update(Exchange exchange);

    /// <summary>
    /// EN: Removes an exchange.
    /// FA: یک بورس حذف می‌کند.
    /// </summary>
    void Remove(Exchange exchange);

    /// <summary>
    /// EN: Gets exchanges with pagination.
    /// FA: بورس‌ها را به صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    Task<PagedResult<Exchange>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
