// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Industry.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents the name of an industry.
/// FA: نام یک صنعت را نمایش می‌دهد.
/// </summary>
public sealed record IndustryName
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="IndustryName"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="IndustryName"/> را ایجاد می‌کند.
    /// </summary>
    public IndustryName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the industry name.
    /// FA: نام صنعت را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static implicit operator string(IndustryName name) => name.Value;
    public static explicit operator IndustryName(string value) => new(value);
}