// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Portfolio.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a portfolio position.
/// FA: شناسه یکتای یک موقعیت سرمایه‌گذاری را نمایش می‌دهد.
/// </summary>
public readonly record struct PositionId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new position identifier.
    /// FA: یک شناسه جدید برای موقعیت ایجاد می‌کند.
    /// </summary>
    public static PositionId New()
        => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString()
        => Value.ToString();
}