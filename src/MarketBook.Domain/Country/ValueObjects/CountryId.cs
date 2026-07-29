// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Country.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Country.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a country.
/// FA: شناسه یکتای یک کشور را نمایش می‌دهد.
/// </summary>
public readonly record struct CountryId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new country identifier.
    /// FA: یک شناسه جدید برای کشور ایجاد می‌کند.
    /// </summary>
    public static CountryId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}