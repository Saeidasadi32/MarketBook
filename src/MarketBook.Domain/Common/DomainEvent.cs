// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents the base class for all domain events.
///
/// FA: کلاس پایه تمام رویدادهای دامنه.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// EN: Initializes a new domain event.
    ///
    /// FA: یک رویداد دامنه جدید ایجاد می‌کند.
    /// </summary>
    protected DomainEvent()
    {
        OccurredOn = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }
}