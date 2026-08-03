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
/// EN: Defines read operations for aggregate repositories.
/// FA: عملیات خواندن مخازن Aggregate را تعریف می‌کند.
/// </summary>
/// <typeparam name="TAggregate">
/// EN: Aggregate type.
/// FA: نوع Aggregate.
/// </typeparam>
/// <typeparam name="TId">
/// EN: Aggregate identifier type.
/// FA: نوع شناسه Aggregate.
/// </typeparam>
public interface IReadRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : EntityId
{
    /// <summary>
    /// EN: Finds an aggregate by identifier.
    /// FA: یک Aggregate را بر اساس شناسه پیدا می‌کند.
    /// </summary>
    Task<TAggregate?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether an aggregate exists.
    /// FA: مشخص می‌کند آیا Aggregate وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default);
}
