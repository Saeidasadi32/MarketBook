// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Instrument.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a financial instrument.
/// FA: شناسه یکتای یک ابزار مالی را نمایش می‌دهد.
/// </summary>
public readonly record struct InstrumentId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new instrument identifier.
    /// FA: یک شناسه جدید برای ابزار مالی ایجاد می‌کند.
    /// </summary>
    public static InstrumentId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}