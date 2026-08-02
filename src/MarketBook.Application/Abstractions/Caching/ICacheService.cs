// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Caching
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Caching;

/// <summary>
/// EN: Defines cache operations.
/// FA: عملیات کش را تعریف می‌کند.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// EN: Gets a cached value by key.
    /// FA: یک مقدار کش‌شده را بر اساس کلید دریافت می‌کند.
    /// </summary>
    /// <typeparam name="T">
    /// EN: Cached value type.
    /// FA: نوع مقدار کش‌شده.
    /// </typeparam>
    /// <param name="key">
    /// EN: Cache key.
    /// FA: کلید کش.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Cached value or null if it does not exist.
    /// FA: مقدار کش‌شده یا null در صورت عدم وجود.
    /// </returns>
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Stores a value in cache.
    /// FA: یک مقدار را در کش ذخیره می‌کند.
    /// </summary>
    /// <typeparam name="T">
    /// EN: Cached value type.
    /// FA: نوع مقدار کش‌شده.
    /// </typeparam>
    /// <param name="key">
    /// EN: Cache key.
    /// FA: کلید کش.
    /// </param>
    /// <param name="value">
    /// EN: Value to cache.
    /// FA: مقداری که باید کش شود.
    /// </param>
    /// <param name="expiration">
    /// EN: Cache expiration time.
    /// FA: زمان انقضای کش.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Removes a cached value.
    /// FA: یک مقدار کش‌شده را حذف می‌کند.
    /// </summary>
    /// <param name="key">
    /// EN: Cache key.
    /// FA: کلید کش.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);
}