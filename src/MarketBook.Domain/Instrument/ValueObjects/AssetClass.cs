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
/// EN: Represents the high-level asset class.
/// FA: کلاس اصلی دارایی را نمایش می‌دهد.
/// </summary>
public sealed record AssetClass
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="AssetClass"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="AssetClass"/> ایجاد می‌کند.
    /// </summary>
    public AssetClass(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets asset class name.
    /// FA: نام کلاس دارایی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    public static readonly AssetClass Equity = new("Equity");
    public static readonly AssetClass ETF = new("ETF");
    public static readonly AssetClass MutualFund = new("Mutual Fund");
    public static readonly AssetClass Bond = new("Bond");
    public static readonly AssetClass Sukuk = new("Sukuk");
    public static readonly AssetClass Commodity = new("Commodity");
    public static readonly AssetClass Currency = new("Currency");
    public static readonly AssetClass Crypto = new("Crypto");
    public static readonly AssetClass Index = new("Index");
    public static readonly AssetClass Derivative = new("Derivative");
    public static readonly AssetClass Other = new("Other");

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
    public static AssetClass FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(AssetClass assetClass)
    {
        ArgumentNullException.ThrowIfNull(assetClass);

        return assetClass.Value;
    }
}
