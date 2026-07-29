// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Instrument.ValueObjects;

/// <summary>
/// EN: Represents an International Securities Identification Number (ISIN).
/// FA: شناسه بین‌المللی اوراق بهادار (ISIN) را نمایش می‌دهد.
/// </summary>
public sealed partial record Isin
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Isin"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Isin"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: ISIN value.
    /// FA: مقدار ISIN.
    /// </param>
    /// <exception cref="ArgumentException">
    /// EN: Thrown when the ISIN format is invalid.
    /// FA: زمانی که قالب ISIN معتبر نباشد.
    /// </exception>
    public Isin(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!IsinRegex.IsMatch(value))
            throw new ArgumentException("Invalid ISIN format.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets the ISIN value.
    /// FA: مقدار ISIN را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    private static readonly Regex IsinRegex =
        new(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]$",
            RegexOptions.Compiled);
}