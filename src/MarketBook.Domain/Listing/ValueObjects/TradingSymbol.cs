// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Listing.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MarketBook.Domain.Listing.ValueObjects;

/// <summary>
/// EN: Represents a trading symbol.
/// FA: نماد معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed partial record TradingSymbol
{
    private static readonly Regex SymbolRegex =
        new(@"^[A-Z0-9._\-]{1,30}$", RegexOptions.Compiled);

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradingSymbol"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TradingSymbol"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Trading symbol.
    /// FA: نماد معاملاتی.
    /// </param>
    public TradingSymbol(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (!SymbolRegex.IsMatch(value))
            throw new ArgumentException("Invalid trading symbol.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets trading symbol.
    /// FA: مقدار نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}