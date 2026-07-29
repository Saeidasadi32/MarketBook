// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Market.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Market.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a market.
/// FA: شناسه یکتای یک بازار را نمایش می‌دهد.
/// </summary>
/// <param name="Value">
/// EN: Underlying ULID value.
/// FA: مقدار ULID.
/// </param>
public readonly record struct MarketId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new market identifier.
    /// FA: یک شناسه جدید برای بازار ایجاد می‌کند.
    /// </summary>
    public static MarketId New() => new(Ulid.NewUlid());

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}