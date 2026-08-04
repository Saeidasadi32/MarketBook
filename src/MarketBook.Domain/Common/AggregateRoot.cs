// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents the root of a Domain-Driven Design aggregate.
///
/// FA: ریشه یک Aggregate در طراحی دامنه را نمایش می‌دهد.
/// </summary>
/// <typeparam name="TId">
/// EN: Aggregate identifier type.
/// FA: نوع شناسه Aggregate.
/// </typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="AggregateRoot{TId}"/> را ایجاد می‌کند.
    /// </summary>
    protected AggregateRoot(TId id) : base(id)
    {
        Version = 0;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    protected AggregateRoot()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the version of the aggregate for concurrency control.
    /// FA: نسخه Aggregate را برای کنترل همزمانی دریافت می‌کند.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// EN: Raises a new domain event.
    ///
    /// FA: یک رویداد دامنه جدید ثبت می‌کند.
    /// </summary>
    protected void Raise(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            throw new DomainException(
                new Error(
                    "AggregateRoot.DomainEvent.Null",
                    "Domain event cannot be null."));

        AddDomainEvent(domainEvent);
        IncrementVersion();
    }

    /// <summary>
    /// EN: Increments the version of the aggregate.
    /// FA: نسخه Aggregate را افزایش می‌دهد.
    /// </summary>
    private void IncrementVersion()
    {
        Version++;
    }
}
