// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common.ValueObjects;

/// <summary>
/// EN: Represents the quantity of a tradable asset.
/// FA: تعداد یا مقدار یک دارایی قابل معامله را نمایش می‌دهد.
/// </summary>
public readonly record struct Quantity :
    IComparable<Quantity>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new quantity.
    /// FA: یک مقدار جدید ایجاد می‌کند.
    /// </summary>
    public Quantity(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    /// <summary>
    /// EN: Gets quantity.
    /// FA: مقدار را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents zero quantity.
    /// FA: مقدار صفر.
    /// </summary>
    public static Quantity Zero => new(0m);

    public bool IsZero => Value == 0m;

    public bool IsPositive => Value > 0m;

    /// <summary>
    /// EN: Adds another quantity.
    /// FA: یک مقدار دیگر را اضافه می‌کند.
    /// </summary>
    public Quantity Add(Quantity other)
        => new(Value + other.Value);

    /// <summary>
    /// EN: Subtracts another quantity.
    /// FA: یک مقدار دیگر را کم می‌کند.
    /// </summary>
    public Quantity Subtract(Quantity other)
        => new(Value - other.Value);

    /// <summary>
    /// EN: Converts quantity to decimal.
    /// FA: مقدار را به decimal تبدیل می‌کند.
    /// </summary>
    public decimal ToDecimal()
        => Value;

    /// <summary>
    /// EN: Creates quantity from decimal.
    /// FA: مقدار را از decimal ایجاد می‌کند.
    /// </summary>
    public static Quantity FromDecimal(decimal value)
        => new(value);

    public int CompareTo(Quantity other)
        => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is Quantity other)
            return CompareTo(other);

        throw new ArgumentException("Object must be of type Quantity.", nameof(obj));
    }

    public override string ToString()
        => Value.ToString("0.########");

    public static Quantity operator +(Quantity left, Quantity right)
        => new(left.Value + right.Value);

    public static Quantity operator -(Quantity left, Quantity right)
        => new(left.Value - right.Value);

    public static implicit operator decimal(Quantity quantity)
        => quantity.Value;

    public static explicit operator Quantity(decimal value)
        => new(value);
}
