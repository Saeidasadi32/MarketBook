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
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Currency aggregate.
/// FA: عملیات مربوط به ذخیره‌سازی Aggregate مربوط به ارز معاملاتی را تعریف می‌کند.
/// </summary>
public interface ICurrencyRepository
{
    /// <summary>
    /// EN: Gets a currency by its identifier.
    /// FA: یک ارز معاملاتی را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Currency identifier.
    /// FA: شناسه ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The currency if found; otherwise null.
    /// FA: در صورت یافتن، ارز معاملاتی و در غیر این صورت null.
    /// </returns>
    Task<Currency?> GetByIdAsync(
        CurrencyId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets a currency by its business code.
    /// FA: یک ارز معاملاتی را بر اساس کد تجاری آن دریافت می‌کند.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The currency if found; otherwise null.
    /// FA: در صورت یافتن، ارز معاملاتی و در غیر این صورت null.
    /// </returns>
    Task<Currency?> GetByCodeAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether a currency with the specified code exists.
    /// FA: بررسی می‌کند آیا ارزی با کد مشخص‌شده وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if a currency with the specified code exists; otherwise false.
    /// FA: اگر ارزی با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    Task<bool> ExistsAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether another currency with the specified code exists,
    /// excluding the specified currency.
    /// FA: بررسی می‌کند آیا ارز معاملاتی دیگری با کد مشخص‌شده،
    /// به‌جز ارز تعیین‌شده، وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="excludingId">
    /// EN: Currency identifier to exclude from the search.
    /// FA: شناسه ارزی که باید از جستجو مستثنی شود.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if another currency with the specified code exists; otherwise false.
    /// FA: اگر ارز دیگری با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    Task<bool> ExistsAsync(
        CurrencyCode code,
        CurrencyId excludingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a new currency to the persistence context.
    /// FA: یک ارز معاملاتی جدید را به Context ذخیره‌سازی اضافه می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to add.
    /// FA: Aggregate ارز معاملاتی برای افزودن.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: An asynchronous operation representing the add action.
    /// FA: عملیات غیرهمزمان مربوط به افزودن ارز معاملاتی.
    /// </returns>
    Task AddAsync(
        Currency currency,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a currency as modified in the persistence context.
    /// FA: یک ارز معاملاتی را به‌عنوان تغییرکرده در Context ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to update.
    /// FA: Aggregate ارز معاملاتی برای به‌روزرسانی.
    /// </param>
    void Update(Currency currency);

    /// <summary>
    /// EN: Marks a currency for removal from persistence.
    /// FA: یک ارز معاملاتی را برای حذف از ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to remove.
    /// FA: Aggregate ارز معاملاتی برای حذف.
    /// </param>
    void Remove(Currency currency);

    /// <summary>
    /// EN: Gets a paged collection of currencys.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از ارزها را دریافت می‌کند.
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
    /// EN: A paged result containing the requested currencys.
    /// FA: نتیجه صفحه‌بندی‌شده شامل ارزها مورد درخواست.
    /// </returns>
    Task<PagedResult<Currency>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
