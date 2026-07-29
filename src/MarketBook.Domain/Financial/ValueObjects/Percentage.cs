// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents a percentage value.
/// FA: یک مقدار درصد را نمایش می‌دهد.
/// </summary>
public readonly record struct Percentage
{
    public Percentage(decimal value)
    {
        Value = decimal.Round(value, 4);

        if (Value < -100m)
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// EN: Gets percentage value.
    /// FA: مقدار درصد را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    public decimal AsFactor()
        => Value / 100m;

    public override string ToString()
        => $"{Value:0.####}%";

    public static implicit operator decimal(Percentage value)
        => value.Value;

    public static explicit operator Percentage(decimal value)
        => new(value);
}