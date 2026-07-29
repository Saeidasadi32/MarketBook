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
/// EN: Represents the base class for all domain entities.
/// An entity is uniquely identified by its identifier rather than its attributes.
///
/// FA: کلاس پایه تمام موجودیت‌های دامنه.
/// هویت یک موجودیت توسط شناسه آن مشخص می‌شود نه مقادیر ویژگی‌های آن.
/// </summary>
/// <typeparam name="TId">
/// EN: Entity identifier type.
/// FA: نوع شناسه موجودیت.
/// </typeparam>
public abstract class Entity<TId>
    where TId : EntityId
{
    /// <summary>
    /// EN: Initializes a new entity.
    ///
    /// FA: یک موجودیت جدید ایجاد می‌کند.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// EN: Gets the unique identifier.
    ///
    /// FA: شناسه یکتای موجودیت.
    /// </summary>
    public TId Id { get; protected init; } = default!;

    /// <summary>
    /// EN: Gets entity version for optimistic concurrency.
    ///
    /// FA: نسخه موجودیت جهت کنترل همزمانی.
    /// </summary>
    public long Version { get; protected set; }

    /// <summary>
    /// EN: Increments entity version.
    ///
    /// FA: نسخه موجودیت را افزایش می‌دهد.
    /// </summary>
    protected void IncrementVersion()
    {
        Version++;
    }

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not Entity<TId> other)
            return false;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <inheritdoc/>
    public sealed override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    /// <summary>
    /// EN: Compares two entities.
    ///
    /// FA: دو موجودیت را مقایسه می‌کند.
    /// </summary>
    public static bool operator ==(
        Entity<TId>? left,
        Entity<TId>? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// EN: Compares two entities.
    ///
    /// FA: دو موجودیت را مقایسه می‌کند.
    /// </summary>
    public static bool operator !=(
        Entity<TId>? left,
        Entity<TId>? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// EN: Returns entity display text.
    ///
    /// FA: متن نمایشی موجودیت.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [{Id}]";
    }
}