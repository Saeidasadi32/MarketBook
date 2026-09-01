// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Exchange.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Exchange.ValueObjects;

/// <summary>
/// EN: Represents the unique business code of an exchange.
/// FA: کد تجاری یکتای یک بورس یا بستر معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed partial record ExchangeCode
{
    private static readonly Regex CodeRegex =
        new(
            @"^[A-Z0-9_-]{2,20}$",
            RegexOptions.Compiled);

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangeCode"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="ExchangeCode"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Exchange business code.
    /// FA: کد تجاری بورس یا بستر معاملاتی.
    /// </param>
    public ExchangeCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(value))
        {
            throw new ArgumentException(
                "Invalid exchange code.",
                nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// EN: Gets the normalized exchange code.
    /// FA: کد نرمال‌شده بورس را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Converts the exchange code to its string representation.
    /// FA: کد بورس را به نمایش رشته‌ای تبدیل می‌کند.
    /// </summary>
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Creates an exchange code from a string.
    /// FA: یک کد بورس را از رشته ایجاد می‌کند.
    /// </summary>
    public static ExchangeCode FromString(string value)
        => new(value);

    /// <summary>
    /// EN: Converts an exchange code implicitly to a string.
    /// FA: کد بورس را به‌صورت ضمنی به رشته تبدیل می‌کند.
    /// </summary>
    public static implicit operator string(ExchangeCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        return code.Value;
    }

    /// <summary>
    /// EN: Converts a string explicitly to an exchange code.
    /// FA: یک رشته را به‌صورت صریح به کد بورس تبدیل می‌کند.
    /// </summary>
    public static explicit operator ExchangeCode(string value)
        => new(value);
}
