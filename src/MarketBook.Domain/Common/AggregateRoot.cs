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
/// FA: ریشه یک Aggregate در طراحی دامنه را نمایش می‌دهد.
/// </summary>
/// <typeparam name="TId">Aggregate identifier type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : EntityId
{
    protected AggregateRoot(TId id)
        : base(id)
    {
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
    /// EN: Gets the version of the aggregate for optimistic concurrency control.
    /// FA: نسخه Aggregate برای کنترل همزمانی خوش‌بینانه.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// EN: Gets whether the aggregate has pending domain events.
    /// FA: مشخص می‌کند Aggregate دارای رویداد دامنه در انتظار انتشار است یا خیر.
    /// </summary>
    public bool HasDomainEvents => DomainEvents.Count > 0;

    /// <summary>
    /// EN: Raises a domain event and advances the aggregate version.
    /// FA: یک رویداد دامنه ثبت کرده و نسخه Aggregate را افزایش می‌دهد.
    /// </summary>
    protected void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        AddDomainEvent(domainEvent);
        Version++;
    }
}
