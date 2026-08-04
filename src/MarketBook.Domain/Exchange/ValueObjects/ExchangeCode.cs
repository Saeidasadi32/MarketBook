// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Exchange.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Country.ValueObjects;
using System.Text.RegularExpressions;

namespace MarketBook.Domain.Exchange.ValueObjects;

/// <summary>
/// EN: Represents an exchange code.
/// FA: کد یک بورس یا صرافی را نمایش می‌دهد.
/// </summary>
public sealed record ExchangeCode
{
    private static readonly Regex Regex =
        new(@"^[A-Z0-9_-]{2,20}$", RegexOptions.Compiled);

    public ExchangeCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(value))
            throw new ArgumentException("Invalid exchange code.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets exchange code.
    /// FA: کد بورس را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Converts Exchange code to string.
    /// FA: کد بورس را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates a Exchange code from string.
    /// FA: کد بورس را از رشته ایجاد می‌کند.
    /// </summary>
    public static ExchangeCode FromString(string value)
        => new(value);

    public override string ToString() => Value;

    public static implicit operator string(ExchangeCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        return code.Value;
    }
}
