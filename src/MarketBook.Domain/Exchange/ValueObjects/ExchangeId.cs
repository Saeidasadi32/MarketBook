// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Exchange.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Exchange.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of an exchange.
/// FA: شناسه یکتای یک بورس یا صرافی را نمایش می‌دهد.
/// </summary>
public readonly record struct ExchangeId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new exchange identifier.
    /// FA: یک شناسه جدید برای بورس یا صرافی ایجاد می‌کند.
    /// </summary>
    public static ExchangeId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}