// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Currency.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Currency.ValueObjects;

/// <summary>
/// EN: Represents an ISO 4217 currency code.
/// FA: کد استاندارد ISO 4217 ارز را نمایش می‌دهد.
/// </summary>
public sealed record CurrencyCode
{
    private static readonly Regex Regex =
        new(@"^[A-Z]{3}$", RegexOptions.Compiled);

    public CurrencyCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(value))
            throw new ArgumentException("Invalid currency code.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets currency code.
    /// FA: کد ارز را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Converts Currency code to string.
    /// FA: کد واحد پولی را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates a Currency code from string.
    /// FA: کد واحد پولی را از رشته ایجاد می‌کند.
    /// </summary>
    public static CurrencyCode FromString(string value)
        => new(value);

    public override string ToString() => Value;

    public static implicit operator string(CurrencyCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        return code.Value;
    }
}
