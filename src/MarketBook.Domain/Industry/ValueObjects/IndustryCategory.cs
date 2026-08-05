// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents the industry of a company (e.g., Automotive, Banking).
/// FA: صنعت یک شرکت را نمایش می‌دهد (مانند خودروسازی، بانکداری).
/// </summary>
public sealed record IndustryCategory
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="IndustryCategory"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="IndustryCategory"/> را ایجاد می‌کند.
    /// </summary>
    public IndustryCategory(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the industry name.
    /// FA: نام صنعت را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Predefined industries.
    /// FA: صنایع از پیش تعریف‌شده.
    /// </summary>
    public static readonly IndustryCategory Automotive = new("Automotive");
    public static readonly IndustryCategory Banking = new("Banking");
    public static readonly IndustryCategory CapitalGoods = new("Capital Goods");
    public static readonly IndustryCategory Chemicals = new("Chemicals");
    public static readonly IndustryCategory Construction = new("Construction");
    public static readonly IndustryCategory ConsumerGoods = new("Consumer Goods");
    public static readonly IndustryCategory Energy = new("Energy");
    public static readonly IndustryCategory FinancialServices = new("Financial Services");
    public static readonly IndustryCategory FoodAndBeverage = new("Food & Beverage");
    public static readonly IndustryCategory Healthcare = new("Healthcare");
    public static readonly IndustryCategory Insurance = new("Insurance");
    public static readonly IndustryCategory Media = new("Media");
    public static readonly IndustryCategory Metals = new("Metals");
    public static readonly IndustryCategory Mining = new("Mining");
    public static readonly IndustryCategory OilAndGas = new("Oil & Gas");
    public static readonly IndustryCategory Pharmaceuticals = new("Pharmaceuticals");
    public static readonly IndustryCategory RealEstate = new("Real Estate");
    public static readonly IndustryCategory Retail = new("Retail");
    public static readonly IndustryCategory Technology = new("Technology");
    public static readonly IndustryCategory Telecommunications = new("Telecommunications");
    public static readonly IndustryCategory Transportation = new("Transportation");
    public static readonly IndustryCategory Utilities = new("Utilities");
    public static readonly IndustryCategory Other = new("Other");

    /// <summary>
    /// EN: Converts industry to string.
    /// FA: صنعت را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates an industry from string.
    /// FA: صنعت را از رشته ایجاد می‌کند.
    /// </summary>
    public static IndustryCategory FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(IndustryCategory industry)
    {
        ArgumentNullException.ThrowIfNull(industry);

        return industry.Value;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator IndustryCategory(string value) => new(value);
}
