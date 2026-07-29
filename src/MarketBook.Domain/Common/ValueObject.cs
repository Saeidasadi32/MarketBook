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
/// EN: Represents the base class for all Value Objects.
/// A Value Object is compared by its values rather than its identity.
///
/// FA: کلاس پایه تمام Value Objectهای دامنه.
/// Value Objectها بر اساس مقادیرشان مقایسه می‌شوند و شناسه مستقل ندارند.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// EN: Returns the values used for equality comparison.
    ///
    /// FA: مقادیر مورد استفاده برای مقایسه را برمی‌گرداند.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null)
            return false;

        if (obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(
                17,
                (current, value) =>
                    current * 31 + (value?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// EN: Equality operator.
    ///
    /// FA: عملگر برابری.
    /// </summary>
    public static bool operator ==(
        ValueObject? left,
        ValueObject? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// EN: Inequality operator.
    ///
    /// FA: عملگر نابرابری.
    /// </summary>
    public static bool operator !=(
        ValueObject? left,
        ValueObject? right)
    {
        return !Equals(left, right);
    }
}