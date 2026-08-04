// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Portfolio.Entities;

/// <summary>
/// EN: Represents the type of an asset in a portfolio.
/// FA: نوع دارایی در یک پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record AssetType
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="AssetType"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="AssetType"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Asset type name.
    /// FA: نام نوع دارایی.
    /// </param>
    public AssetType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the asset type name.
    /// FA: نام نوع دارایی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Predefined asset type: Stock (Equity).
    /// FA: نوع دارایی از پیش تعریف‌شده: سهام.
    /// </summary>
    public static readonly AssetType Stock = new("Stock");

    /// <summary>
    /// EN: Predefined asset type: ETF.
    /// FA: نوع دارایی از پیش تعریف‌شده: صندوق قابل معامله.
    /// </summary>
    public static readonly AssetType ETF = new("ETF");

    /// <summary>
    /// EN: Predefined asset type: Mutual Fund.
    /// FA: نوع دارایی از پیش تعریف‌شده: صندوق سرمایه‌گذاری.
    /// </summary>
    public static readonly AssetType MutualFund = new("MutualFund");

    /// <summary>
    /// EN: Predefined asset type: Bond.
    /// FA: نوع دارایی از پیش تعریف‌شده: اوراق قرضه.
    /// </summary>
    public static readonly AssetType Bond = new("Bond");

    /// <summary>
    /// EN: Predefined asset type: Sukuk (Islamic bond).
    /// FA: نوع دارایی از پیش تعریف‌شده: صکوک.
    /// </summary>
    public static readonly AssetType Sukuk = new("Sukuk");

    /// <summary>
    /// EN: Predefined asset type: Commodity.
    /// FA: نوع دارایی از پیش تعریف‌شده: کالا.
    /// </summary>
    public static readonly AssetType Commodity = new("Commodity");

    /// <summary>
    /// EN: Predefined asset type: Currency.
    /// FA: نوع دارایی از پیش تعریف‌شده: ارز.
    /// </summary>
    public static readonly AssetType Currency = new("Currency");

    /// <summary>
    /// EN: Predefined asset type: Cryptocurrency.
    /// FA: نوع دارایی از پیش تعریف‌شده: رمزارز.
    /// </summary>
    public static readonly AssetType Crypto = new("Crypto");

    /// <summary>
    /// EN: Predefined asset type: Derivative.
    /// FA: نوع دارایی از پیش تعریف‌شده: مشتقه.
    /// </summary>
    public static readonly AssetType Derivative = new("Derivative");

    /// <summary>
    /// EN: Predefined asset type: Index.
    /// FA: نوع دارایی از پیش تعریف‌شده: شاخص.
    /// </summary>
    public static readonly AssetType Index = new("Index");

    /// <summary>
    /// EN: Predefined asset type: Other.
    /// FA: نوع دارایی از پیش تعریف‌شده: سایر.
    /// </summary>
    public static readonly AssetType Other = new("Other");

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
    public static AssetType FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(AssetType assetType)
    {
        ArgumentNullException.ThrowIfNull(assetType);

        return assetType.Value;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator AssetType(string value)
        => new(value);
}
