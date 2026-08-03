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
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Represents the application's database context abstraction.
/// FA: انتزاعی از پایگاه داده برنامه را نمایش می‌دهد.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// EN: Gets a set for the specified entity type.
    /// FA: مجموعه موجودیت از نوع مشخص‌شده را دریافت می‌کند.
    /// </summary>
    /// <typeparam name="TEntity">
    /// EN: Entity type.
    /// FA: نوع موجودیت.
    /// </typeparam>
    /// <returns>
    /// EN: Entity set.
    /// FA: مجموعه موجودیت.
    /// </returns>
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    /// <summary>
    /// EN: Gets an entry for the specified entity.
    /// FA: ورودی مربوط به موجودیت مشخص‌شده را دریافت می‌کند.
    /// </summary>
    /// <typeparam name="TEntity">
    /// EN: Entity type.
    /// FA: نوع موجودیت.
    /// </typeparam>
    /// <param name="entity">
    /// EN: Entity instance.
    /// FA: نمونه موجودیت.
    /// </param>
    /// <returns>
    /// EN: Entity entry.
    /// FA: ورودی موجودیت.
    /// </returns>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// EN: Saves all changes asynchronously.
    /// FA: تمام تغییرات را به صورت ناهمزمان ذخیره می‌کند.
    /// </summary>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Number of affected records.
    /// FA: تعداد رکوردهای تحت تأثیر.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
