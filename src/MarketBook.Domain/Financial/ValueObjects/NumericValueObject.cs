// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents the base class for numeric value objects.
///
/// FA: کلاس پایه تمام ValueObjectهای عددی.
/// </summary>
/// <typeparam name="TValue">
/// EN: Numeric type.
/// FA: نوع عددی.
/// </typeparam>
public abstract class NumericValueObject<TValue> : ValueObject
    where TValue : struct, IComparable<TValue>
{
    protected NumericValueObject(TValue value)
    {
        Value = value;
    }

    /// <summary>
    /// EN: Gets numeric value.
    ///
    /// FA: مقدار عددی.
    /// </summary>
    public TValue Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
        => Value.ToString();
}