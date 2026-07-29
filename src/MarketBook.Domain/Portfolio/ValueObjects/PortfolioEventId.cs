// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.ValueObjects
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.Portfolio.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a transaction.
/// FA: شناسه یکتای یک معامله را نمایش می‌دهد.
/// </summary>
public readonly record struct PortfolioEventId(Ulid Value)
{
    public static PortfolioEventId New()
        => new(Ulid.NewUlid());

    public override string ToString()
        => Value.ToString();
}