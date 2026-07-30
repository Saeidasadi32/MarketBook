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
    private readonly List<IDomainEvent> _domainEvents = [];

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
    /// EN: Gets all pending domain events.
    ///
    /// FA: تمام رویدادهای دامنه ثبت‌شده را دریافت می‌کند.
    /// </summary>
    public new IReadOnlyCollection<IDomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    /// <summary>
    /// EN: Gets the version of the aggregate for concurrency control.
    /// FA: نسخه Aggregate را برای کنترل همزمانی دریافت می‌کند.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// EN: Determines whether the aggregate has pending events.
    ///
    /// FA: مشخص می‌کند آیا Aggregate رویداد ثبت‌شده دارد یا خیر.
    /// </summary>
    public bool HasDomainEvents
        => _domainEvents.Count > 0;

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

        _domainEvents.Add(domainEvent);
        IncrementVersion();
    }

    /// <summary>
    /// EN: Removes all pending domain events.
    ///
    /// FA: تمام رویدادهای دامنه را پاک می‌کند.
    /// </summary>
    public new void ClearDomainEvents()
    {
        _domainEvents.Clear();
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