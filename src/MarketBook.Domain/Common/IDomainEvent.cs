// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents a domain event.
///
/// FA: یک رویداد دامنه را نمایش می‌دهد.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// EN: Gets the occurrence date and time.
    ///
    /// FA: زمان وقوع رویداد.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}