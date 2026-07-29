// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Instrument.ValueObjects;

/// <summary>
/// EN: Represents the display name of a financial instrument.
/// FA: نام نمایشی ابزار مالی را نمایش می‌دهد.
/// </summary>
public sealed record InstrumentName
{
    public InstrumentName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the instrument name.
    /// FA: نام ابزار مالی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    public override string ToString() => Value;
}