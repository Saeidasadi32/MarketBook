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
public sealed record Industry
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Industry"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Industry"/> را ایجاد می‌کند.
    /// </summary>
    public Industry(string value)
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
    public static readonly Industry Automotive = new("Automotive");
    public static readonly Industry Banking = new("Banking");
    public static readonly Industry CapitalGoods = new("Capital Goods");
    public static readonly Industry Chemicals = new("Chemicals");
    public static readonly Industry Construction = new("Construction");
    public static readonly Industry ConsumerGoods = new("Consumer Goods");
    public static readonly Industry Energy = new("Energy");
    public static readonly Industry FinancialServices = new("Financial Services");
    public static readonly Industry FoodAndBeverage = new("Food & Beverage");
    public static readonly Industry Healthcare = new("Healthcare");
    public static readonly Industry Insurance = new("Insurance");
    public static readonly Industry Media = new("Media");
    public static readonly Industry Metals = new("Metals");
    public static readonly Industry Mining = new("Mining");
    public static readonly Industry OilAndGas = new("Oil & Gas");
    public static readonly Industry Pharmaceuticals = new("Pharmaceuticals");
    public static readonly Industry RealEstate = new("Real Estate");
    public static readonly Industry Retail = new("Retail");
    public static readonly Industry Technology = new("Technology");
    public static readonly Industry Telecommunications = new("Telecommunications");
    public static readonly Industry Transportation = new("Transportation");
    public static readonly Industry Utilities = new("Utilities");
    public static readonly Industry Other = new("Other");

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(Industry industry) => industry.Value;

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator Industry(string value) => new(value);
}