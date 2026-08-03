// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines write operations for aggregate repositories.
/// FA: عملیات نوشتنی مخازن Aggregate را تعریف می‌کند.
/// </summary>
/// <typeparam name="TAggregate">
/// EN: Aggregate type.
/// FA: نوع Aggregate.
/// </typeparam>
/// <typeparam name="TId">
/// EN: Aggregate identifier type.
/// FA: نوع شناسه Aggregate.
/// </typeparam>
public interface IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : EntityId
{
    /// <summary>
    /// EN: Adds an aggregate.
    /// FA: یک Aggregate را اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Removes an aggregate.
    /// FA: یک Aggregate را حذف می‌کند.
    /// </summary>
    void Remove(TAggregate aggregate);

    /// <summary>
    /// EN: Updates an aggregate.
    /// FA: یک Aggregate را به‌روزرسانی می‌کند.
    /// </summary>
    void Update(TAggregate aggregate);
}
