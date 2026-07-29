// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Country.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Country.ValueObjects;

/// <summary>
/// EN: Represents an ISO 3166-1 Alpha-2 country code.
/// FA: کد استاندارد ISO 3166-1 Alpha-2 کشور را نمایش می‌دهد.
/// </summary>
public sealed record CountryCode
{
    private static readonly Regex Regex =
        new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CountryCode"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CountryCode"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: ISO country code.
    /// FA: کد استاندارد کشور.
    /// </param>
    public CountryCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(value))
            throw new ArgumentException("Invalid country code.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets the ISO country code.
    /// FA: کد استاندارد کشور را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    public static implicit operator string(CountryCode code) => code.Value;
}