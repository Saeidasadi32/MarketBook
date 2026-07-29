// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents percentage change.
/// FA: درصد تغییر را نمایش می‌دهد.
/// </summary>
public readonly record struct PercentageChange
{
    public PercentageChange(Percentage value)
    {
        Value = value;
    }

    public Percentage Value { get; }

    public override string ToString()
        => Value.ToString();
}