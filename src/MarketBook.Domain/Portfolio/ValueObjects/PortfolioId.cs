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
/// EN: Represents the unique identifier of a portfolio.
/// FA: شناسه یکتای یک پرتفوی را نمایش می‌دهد.
/// </summary>
/// <param name="Value">
/// EN: The underlying ULID value.
/// FA: مقدار ULID شناسه.
/// </param>
public readonly record struct PortfolioId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new unique portfolio identifier.
    /// FA: یک شناسه یکتای جدید برای پرتفوی ایجاد می‌کند.
    /// </summary>
    public static PortfolioId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}