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
/// EN: Represents a monetary price.
/// FA: یک مقدار قیمت را نمایش می‌دهد.
/// </summary>
public readonly record struct Price
{
    /// <summary>
    /// EN: Initializes a new price.
    /// FA: یک قیمت جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Price value.
    /// FA: مقدار قیمت.
    /// </param>
    public Price(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = decimal.Round(value, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets price value.
    /// FA: مقدار قیمت را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents a zero price.
    /// FA: قیمت صفر را نمایش می‌دهد.
    /// </summary>
    public static Price Zero => new(0m);

    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString("0.######");

    public static implicit operator decimal(Price price)
        => price.Value;

    public static explicit operator Price(decimal value)
        => new(value);

    public static Price operator +(Price left, Price right)
        => new(left.Value + right.Value);

    public static Price operator -(Price left, Price right)
        => new(left.Value - right.Value);

    public static bool operator >(Price left, Price right)
        => left.Value > right.Value;

    public static bool operator <(Price left, Price right)
        => left.Value < right.Value;

    public static bool operator >=(Price left, Price right)
        => left.Value >= right.Value;

    public static bool operator <=(Price left, Price right)
        => left.Value <= right.Value;
}