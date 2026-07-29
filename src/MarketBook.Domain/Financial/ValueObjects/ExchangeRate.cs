// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents a currency exchange rate.
/// FA: نرخ تبدیل ارز را نمایش می‌دهد.
/// </summary>
public readonly record struct ExchangeRate
{
    public ExchangeRate(Price value)
    {
        if (value.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public Price Value { get; }

    public override string ToString()
        => Value.ToString();
}