// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents a domain exception.
/// FA: استثناء مربوط به دامنه.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// EN: Initializes exception.
    /// FA: ایجاد استثناء.
    /// </summary>
    public DomainException(Error error)
        : base(error.Message)
    {
        Error = error;
    }

    /// <summary>
    /// EN: Gets domain error.
    /// FA: خطای دامنه.
    /// </summary>
    public Error Error { get; }
}