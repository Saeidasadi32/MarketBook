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

    /// <summary>
    /// EN: Converts the value object to string.
    /// FA: مقدار شیء را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates an industry name from string.
    /// FA: نام صنعت را از رشته ایجاد می‌کند.
    /// </summary>
    public static IndustryName FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(IndustryName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return name.Value;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator IndustryName(string value)
        => FromString(value);
}
