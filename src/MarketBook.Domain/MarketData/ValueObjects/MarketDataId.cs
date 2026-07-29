// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.MarketData.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of market data.
/// FA: شناسه یکتای داده بازار را نمایش می‌دهد.
/// </summary>
public readonly record struct MarketDataId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new identifier.
    /// FA: یک شناسه جدید ایجاد می‌کند.
    /// </summary>
    public static MarketDataId New() => new(Ulid.NewUlid());

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}