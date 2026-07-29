// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents a mathematical ratio.
/// FA: یک نسبت ریاضی را نمایش می‌دهد.
/// </summary>
public readonly record struct Ratio
{
    public Ratio(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public decimal Value { get; }

    public override string ToString()
        => Value.ToString("0.######");
}