// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Represents the Unit of Work for coordinating persistence operations.
/// FA: واحد کار برای هماهنگ‌سازی عملیات ذخیره‌سازی را نمایش می‌دهد.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// EN: Persists all pending changes.
    /// FA: تمام تغییرات در انتظار را ذخیره می‌کند.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}