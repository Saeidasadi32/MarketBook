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
/// The aggregate root is responsible for protecting consistency boundaries
/// and managing domain events.
///
/// FA: ریشه یک Aggregate در طراحی دامنه را نمایش می‌دهد.
/// ریشه Aggregate مسئول حفظ یکپارچگی دامنه و مدیریت رویدادهای دامنه است.
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
    /// EN: Gets all pending domain events.
    ///
    /// FA: تمام رویدادهای دامنه ثبت‌شده را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    /// <summary>
    /// EN: Raises a new domain event.
    ///
    /// FA: یک رویداد دامنه جدید ثبت می‌کند.
    /// </summary>
    /// <param name="domainEvent">
    /// EN: Domain event.
    ///
    /// FA: رویداد دامنه.
    /// </param>
    protected void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);

        IncrementVersion();
    }

    /// <summary>
    /// EN: Removes all pending domain events.
    ///
    /// FA: تمام رویدادهای دامنه را پاک می‌کند.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// EN: Determines whether the aggregate has pending events.
    ///
    /// FA: مشخص می‌کند آیا Aggregate رویداد ثبت‌شده دارد یا خیر.
    /// </summary>
    public bool HasDomainEvents
        => _domainEvents.Count > 0;
}