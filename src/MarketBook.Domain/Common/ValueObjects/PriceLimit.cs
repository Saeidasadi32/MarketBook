// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common.ValueObjects
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common.ValueObjects;

/// <summary>
/// EN: Represents a daily price limit.
/// FA: محدوده مجاز قیمت روزانه را نمایش می‌دهد.
/// </summary>
public readonly record struct PriceLimit
{
    public PriceLimit(Price value)
    {
        Value = value;
    }

    /// <summary>
    /// EN: Gets limit price.
    /// FA: قیمت حد را دریافت می‌کند.
    /// </summary>
    public Price Value { get; }

    public override string ToString()
        => Value.ToString();
}