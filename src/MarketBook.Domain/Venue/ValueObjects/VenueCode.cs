
// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Domain.Venue.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using System.Text.RegularExpressions;
namespace MarketBook.Domain.Venue.ValueObjects;

/// <summary>
/// EN: Represents the unique business code of a trading venue.
/// FA: کد تجاری یکتای یک بستر معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed partial record VenueCode
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="VenueCode"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="VenueCode"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Venue code value.
    /// FA: مقدار کد بستر معاملاتی.
    /// </param>
    public VenueCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!VenueCodeRegex.IsMatch(value))
        {
            throw new ArgumentException(
                "Invalid venue code format.",
                nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// EN: Gets the venue code value.
    /// FA: مقدار کد بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Returns the string representation of the venue code.
    /// FA: نمایش رشته‌ای کد بستر معاملاتی را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Creates a venue code from a string.
    /// FA: یک کد بستر معاملاتی را از رشته ایجاد می‌کند.
    /// </summary>
    public static VenueCode FromString(string value)
        => new(value);

    /// <summary>
    /// EN: Converts a venue code implicitly to a string.
    /// FA: کد بستر معاملاتی را به‌صورت ضمنی به رشته تبدیل می‌کند.
    /// </summary>
    public static implicit operator string(VenueCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        return code.Value;
    }

    /// <summary>
    /// EN: Converts a string explicitly to a venue code.
    /// FA: یک رشته را به‌صورت صریح به کد بستر معاملاتی تبدیل می‌کند.
    /// </summary>
    public static explicit operator VenueCode(string value)
        => new(value);

    /// <summary>
    /// EN: Converts a string explicitly to a venue code.
    /// FA: یک رشته را به‌صورت صریح به کد بستر معاملاتی تبدیل می‌کند.
    /// </summary>
    private static readonly Regex VenueCodeRegex =
        new(
            @"^[A-Z0-9_-]{2,20}$",
            RegexOptions.Compiled);
}
