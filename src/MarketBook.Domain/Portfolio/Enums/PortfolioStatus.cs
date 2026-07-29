// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Enums
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Portfolio.Enums;

/// <summary>
/// EN: Represents the current portfolio status.
/// FA: وضعیت فعلی پرتفوی را نمایش می‌دهد.
/// </summary>
public enum PortfolioStatus
{
    Active = 1,
    Archived = 2,
    Closed = 3
}