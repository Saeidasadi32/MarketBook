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
/// EN: Represents a trading volume.
/// FA: حجم معاملات را نمایش می‌دهد.
/// </summary>
public readonly record struct Volume
{
    /// <summary>
    /// EN: Initializes a new volume.
    /// FA: یک حجم جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Volume value.
    /// FA: مقدار حجم.
    /// </param>
    public Volume(long value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets volume value.
    /// FA: مقدار حجم را دریافت می‌کند.
    /// </summary>
    public long Value { get; }

    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString("N0");

    public static implicit operator long(Volume value)
        => value.Value;

    public static explicit operator Volume(long value)
        => new(value);

    public static Volume operator +(Volume left, Volume right)
        => new(left.Value + right.Value);

    public static Volume operator -(Volume left, Volume right)
        => new(left.Value - right.Value);
}