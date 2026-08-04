// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Sector.ValueObjects;

/// <summary>
/// EN: Represents the economic sector of a company.
/// FA: بخش اقتصادی یک شرکت را نمایش می‌دهد.
/// </summary>
public sealed record Sector
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Sector"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Sector"/> را ایجاد می‌کند.
    /// </summary>
    public Sector(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the sector name.
    /// FA: نام بخش را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Predefined sectors (GICS standard).
    /// FA: بخش‌های از پیش تعریف‌شده (استاندارد GICS).
    /// </summary>
    public static readonly Sector CommunicationServices = new("Communication Services");
    public static readonly Sector ConsumerDiscretionary = new("Consumer Discretionary");
    public static readonly Sector ConsumerStaples = new("Consumer Staples");
    public static readonly Sector Energy = new("Energy");
    public static readonly Sector Financials = new("Financials");
    public static readonly Sector Healthcare = new("Healthcare");
    public static readonly Sector Industrials = new("Industrials");
    public static readonly Sector InformationTechnology = new("Information Technology");
    public static readonly Sector Materials = new("Materials");
    public static readonly Sector RealEstate = new("Real Estate");
    public static readonly Sector Utilities = new("Utilities");

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
    public static Sector FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(Sector sector)
    {
        ArgumentNullException.ThrowIfNull(sector);

        return sector.Value;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator Sector(string value) => new(value);
}
