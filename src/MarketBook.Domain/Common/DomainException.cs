
// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents an exception that occurs when a domain rule is violated.
/// FA: استثنایی که هنگام نقض یک قانون دامنه رخ می‌دهد را نمایش می‌دهد.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="DomainException"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="DomainException"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="error">
    /// EN: The error containing the reason for the exception.
    /// FA: خطای حاوی دلیل ایجاد استثنا.
    /// </param>
    public DomainException(Error error)
        : base(error.Message)
    {
        Error = error;
    }

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="DomainException"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="DomainException"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="error">
    /// EN: The error containing the reason for the exception.
    /// FA: خطای حاوی دلیل ایجاد استثنا.
    /// </param>
    /// <param name="innerException">
    /// EN: The inner exception that caused this exception.
    /// FA: استثنای داخلی که باعث این استثنا شده است.
    /// </param>
    public DomainException(Error error, Exception innerException)
        : base(error.Message, innerException)
    {
        Error = error;
    }

    /// <summary>
    /// EN: Gets the error associated with this exception.
    /// FA: خطای مرتبط با این استثنا را دریافت می‌کند.
    /// </summary>
    public Error Error { get; }
}