// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Currency.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Currency.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a currency.
/// FA: شناسه یکتای یک ارز را نمایش می‌دهد.
/// </summary>
public readonly record struct CurrencyId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new currency identifier.
    /// FA: یک شناسه جدید برای ارز ایجاد می‌کند.
    /// </summary>
    public static CurrencyId New() => new(Ulid.NewUlid());

    /// <inheritdoc/>
    public override string ToString() => Value.ToString();
}