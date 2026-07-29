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
/// EN: Represents the number of executed trades.
/// FA: تعداد معاملات انجام شده را نمایش می‌دهد.
/// </summary>
public readonly record struct TradeCount
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradeCount"/> struct.
    /// FA: یک نمونه جدید از <see cref="TradeCount"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Number of trades.
    /// FA: تعداد معاملات.
    /// </param>
    public TradeCount(long value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets the number of trades.
    /// FA: تعداد معاملات را دریافت می‌کند.
    /// </summary>
    public long Value { get; }

    public override string ToString() => Value.ToString("N0");

    public static implicit operator long(TradeCount value) => value.Value;

    public static explicit operator TradeCount(long value) => new(value);
}