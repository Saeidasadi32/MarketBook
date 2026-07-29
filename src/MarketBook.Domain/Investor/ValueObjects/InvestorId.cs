// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Investor.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Investor.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of an investor.
/// FA: شناسه یکتای سرمایه‌گذار را نمایش می‌دهد.
/// </summary>
/// <param name="Value">
/// EN: Underlying ULID value.
/// FA: مقدار ULID.
/// </param>
public readonly record struct InvestorId(Ulid Value)
{
    /// <summary>
    /// EN: Creates a new investor identifier.
    /// FA: یک شناسه جدید برای سرمایه‌گذار ایجاد می‌کند.
    /// </summary>
    public static InvestorId New() => new(Ulid.NewUlid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}