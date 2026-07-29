// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.ValueObjects
// -----------------------------------------------------------------------------

using NUlid;

namespace MarketBook.Domain.CorporateActions.ValueObjects;

/// <summary>
/// EN: Unique corporate action identifier.
/// FA: شناسه یکتای رویداد شرکتی.
/// </summary>
public readonly record struct CorporateActionId(Ulid Value)
{
    public static CorporateActionId New()
        => new(Ulid.NewUlid());

    public override string ToString()
        => Value.ToString();
}