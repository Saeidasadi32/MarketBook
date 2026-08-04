// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Instrument.ValueObjects;

/// <summary>
/// EN: Represents the high-level category of a financial instrument.
/// FA: گروه اصلی یک ابزار مالی را نمایش می‌دهد.
/// </summary>
public sealed record InstrumentCategory
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="InstrumentCategory"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="InstrumentCategory"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Category name.
    /// FA: نام گروه.
    /// </param>
    public InstrumentCategory(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets category value.
    /// FA: مقدار گروه را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    public static readonly InstrumentCategory Equity = new("Equity");
    public static readonly InstrumentCategory Derivative = new("Derivative");
    public static readonly InstrumentCategory Fund = new("Fund");
    public static readonly InstrumentCategory Commodity = new("Commodity");
    public static readonly InstrumentCategory Currency = new("Currency");
    public static readonly InstrumentCategory Crypto = new("Crypto");
    public static readonly InstrumentCategory FixedIncome = new("FixedIncome");
    public static readonly InstrumentCategory Index = new("Index");
    public static readonly InstrumentCategory Other = new("Other");

    /// <summary>
    /// EN: Converts the value object to string.
    /// FA: مقدار شیء را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates asset class from string.
    /// FA: کلاس دارایی را از رشته ایجاد می‌کند.
    /// </summary>
    public static InstrumentCategory FromString(string value)
        => new(value);


    public override string ToString() => Value;

    public static implicit operator string(InstrumentCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return category.Value;
    }
}
