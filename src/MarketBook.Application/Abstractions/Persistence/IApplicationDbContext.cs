// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Represents the application's database context contract.
/// FA: قرارداد Context پایگاه داده برنامه را نمایش می‌دهد.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// EN: Persists all changes to the database.
    /// FA: تمام تغییرات را در پایگاه داده ذخیره می‌کند.
    /// </summary>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Number of affected rows.
    /// FA: تعداد رکوردهای تغییر یافته.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}