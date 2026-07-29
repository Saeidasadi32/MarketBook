// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Listing.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Listing.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a listing.
/// FA: شناسه یکتای یک پذیرش معاملاتی را نمایش می‌دهد.
/// </summary>
/// <param name="Value">
/// EN: Underlying ULID value.
/// FA: مقدار ULID.
/// </param>
public readonly record struct ListingId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new listing identifier.
    /// FA: یک شناسه جدید برای پذیرش معاملاتی ایجاد می‌کند.
    /// </summary>
    public static ListingId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}