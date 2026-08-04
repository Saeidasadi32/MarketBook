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
/// EN: Represents the base type for all domain entities with a specific identifier type.
/// FA: کلاس پایه برای تمام موجودیت‌های دامنه با نوع شناسه مشخص.
/// </summary>
/// <typeparam name="TId">
/// EN: The type of the entity identifier. Must inherit from EntityId.
/// FA: نوع شناسه موجودیت. باید از EntityId ارث‌برداری کند.
/// </typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// EN: Gets the unique identifier of the entity.
    /// FA: شناسه یکتای موجودیت را برمی‌گرداند.
    /// </summary>
    public TId Id { get; protected init; } = default!;

    /// <summary>
    /// EN: Gets a read-only collection of domain events that occurred during the entity's lifecycle.
    /// FA: مجموعه فقط‌خواندنی از رویدادهای دامنه که در طول چرخه‌ حیات موجودیت رخ داده‌اند را برمی‌گرداند.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// EN: Initializes a new instance of the Entity class with the specified identifier.
    /// FA: یک نمونه جدید از کلاس Entity با شناسه مشخص شده مقداردهی اولیه می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: The unique identifier for the entity.
    /// FA: شناسه یکتای موجودیت.
    /// </param>
    /// <exception cref="DomainException">
    /// EN: Thrown when the provided identifier is null.
    /// FA: زمانی که شناسه ارسالی null باشد پرتاب می‌شود.
    /// </exception>
    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Id = id;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    protected Entity()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Adds a domain event to the entity's event collection.
    /// FA: یک رویداد دامنه به مجموعه رویدادهای موجودیت اضافه می‌کند.
    /// </summary>
    /// <param name="domainEvent">
    /// EN: The domain event to add.
    /// FA: رویداد دامنه‌ای که باید اضافه شود.
    /// </param>
    /// <exception cref="DomainException">
    /// EN: Thrown when the domain event is null.
    /// FA: زمانی که رویداد دامنه null باشد پرتاب می‌شود.
    /// </exception>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// EN: Removes a specific domain event from the entity's event collection.
    /// FA: یک رویداد دامنه مشخص را از مجموعه رویدادهای موجودیت حذف می‌کند.
    /// </summary>
    /// <param name="domainEvent">
    /// EN: The domain event to remove.
    /// FA: رویداد دامنه‌ای که باید حذف شود.
    /// </param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            return;

        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// EN: Clears all domain events from the entity's event collection.
    /// FA: تمام رویدادهای دامنه را از مجموعه رویدادهای موجودیت پاک می‌کند.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// EN: Determines whether the specified entity is equal to the current entity.
    /// FA: تعیین می‌کند که آیا موجودیت مشخص شده با موجودیت جاری برابر است یا خیر.
    /// </summary>
    /// <param name="other">
    /// EN: The entity to compare with the current entity.
    /// FA: موجودیتی که با موجودیت جاری مقایسه می‌شود.
    /// </param>
    /// <returns>
    /// EN: True if the specified entity is equal to the current entity; otherwise, false.
    /// FA: در صورت برابری موجودیت مشخص شده با موجودیت جاری true و در غیر این صورت false.
    /// </returns>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// EN: Determines whether the specified object is equal to the current entity.
    /// FA: تعیین می‌کند که آیا شیء مشخص شده با موجودیت جاری برابر است یا خیر.
    /// </summary>
    /// <param name="obj">
    /// EN: The object to compare with the current entity.
    /// FA: شیئی که با موجودیت جاری مقایسه می‌شود.
    /// </param>
    /// <returns>
    /// EN: True if the specified object is equal to the current entity; otherwise, false.
    /// FA: در صورت برابری شیء مشخص شده با موجودیت جاری true و در غیر این صورت false.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        if (obj is not Entity<TId> other)
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// EN: Serves as the default hash function.
    /// FA: به عنوان تابع هش پیش‌فرض عمل می‌کند.
    /// </summary>
    /// <returns>
    /// EN: A hash code for the current entity.
    /// FA: کد هش برای موجودیت جاری.
    /// </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }

    /// <summary>
    /// EN: Returns a string representation of the entity.
    /// FA: نمایش رشته‌ای از موجودیت را برمی‌گرداند.
    /// </summary>
    /// <returns>
    /// EN: A string containing the entity type and its identifier.
    /// FA: رشته‌ای حاوی نوع موجودیت و شناسه آن.
    /// </returns>
    public override string ToString()
    {
        return $"{GetType().Name} [Id = {Id}]";
    }

    /// <summary>
    /// EN: Determines whether two entities are equal.
    /// FA: تعیین می‌کند که آیا دو موجودیت برابر هستند یا خیر.
    /// </summary>
    /// <param name="left">
    /// EN: The first entity to compare.
    /// FA: اولین موجودیت برای مقایسه.
    /// </param>
    /// <param name="right">
    /// EN: The second entity to compare.
    /// FA: دومین موجودیت برای مقایسه.
    /// </param>
    /// <returns>
    /// EN: True if the two entities are equal; otherwise, false.
    /// FA: در صورت برابری دو موجودیت true و در غیر این صورت false.
    /// </returns>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// EN: Determines whether two entities are not equal.
    /// FA: تعیین می‌کند که آیا دو موجودیت نابرابر هستند یا خیر.
    /// </summary>
    /// <param name="left">
    /// EN: The first entity to compare.
    /// FA: اولین موجودیت برای مقایسه.
    /// </param>
    /// <param name="right">
    /// EN: The second entity to compare.
    /// FA: دومین موجودیت برای مقایسه.
    /// </param>
    /// <returns>
    /// EN: True if the two entities are not equal; otherwise, false.
    /// FA: در صورت نابرابری دو موجودیت true و در غیر این صورت false.
    /// </returns>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}
