// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Market.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Market.ValueObjects;

/// <summary>
/// EN: Represents the unique code of a financial market.
/// FA: کد یکتای یک بازار مالی را نمایش می‌دهد.
/// </summary>
public sealed partial record MarketCode
{
    private static readonly Regex CodeRegex =
        new(@"^[A-Z0-9_-]{2,20}$", RegexOptions.Compiled);

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="MarketCode"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="MarketCode"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Market code.
    /// FA: کد بازار.
    /// </param>
    public MarketCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(value))
            throw new ArgumentException("Invalid market code.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets the market code.
    /// FA: کد بازار را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Converts the market code to its string representation.
    /// FA: کد بازار را به نمایش رشته‌ای تبدیل می‌کند.
    /// </summary>
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Creates asset class from string.
    /// FA: کلاس دارایی را از رشته ایجاد می‌کند.
    /// </summary>
    public static MarketCode FromString(string value)
        => new(value);

    /// <summary>
    /// EN: Converts the market code implicitly to a string.
    /// FA: کد بازار را به‌صورت ضمنی به رشته تبدیل می‌کند.
    /// </summary>
    public static implicit operator string(MarketCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        return code.Value;
    }
}
