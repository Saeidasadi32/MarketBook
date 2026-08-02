// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Common.Interfaces.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Represents a unit of work for coordinating persistence operations.
///
/// FA: واحد کار برای هماهنگ‌سازی عملیات ذخیره‌سازی را نمایش می‌دهد.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// EN: Persists all pending changes asynchronously.
    ///
    /// FA: تمام تغییرات در انتظار را به صورت ناهمگام ذخیره می‌کند.
    /// </summary>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    ///
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Number of affected records.
    ///
    /// FA: تعداد رکوردهای تحت تأثیر.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}