// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.ValueObjects
// -----------------------------------------------------------------------------

using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Portfolio.ValueObjects;

/// <summary>
/// EN: Represents a portfolio name.
/// FA: نام پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record PortfolioName
{
    public const int MaxLength = 100;

    public PortfolioName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    /// <summary>
    /// EN: Gets portfolio name.
    /// FA: نام پرتفوی.
    /// </summary>
    public string Value { get; }

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
    public static PortfolioName FromString(string value)
        => new(value);

    public override string ToString() => Value;

    public static implicit operator string(PortfolioName value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Value;
    }

    public static explicit operator PortfolioName(string value)
        => new(value);
}
