// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Exceptions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;

namespace MarketBook.Application.Exceptions;

/// <summary>
/// EN: Represents the base exception for the Application layer.
/// FA: استثنای پایه برای لایه Application را نمایش می‌دهد.
/// </summary>
public class ApplicationException : Exception
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ApplicationException"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ApplicationException"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="error">
    /// EN: Application error.
    /// FA: خطای برنامه.
    /// </param>
    public ApplicationException(Error error)
        : base(GetMessage(error))
    {
        Error = error;
    }

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ApplicationException"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ApplicationException"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="error">
    /// EN: Application error.
    /// FA: خطای برنامه.
    /// </param>
    /// <param name="innerException">
    /// EN: Inner exception.
    /// FA: استثنای داخلی.
    /// </param>
    public ApplicationException(
        Error error,
        Exception innerException)
        : base(GetMessage(error), innerException)
    {
        ArgumentNullException.ThrowIfNull(innerException);
        Error = error;
    }

    /// <summary>
    /// EN: Gets the associated error.
    /// FA: خطای مرتبط را دریافت می‌کند.
    /// </summary>
    public Error Error { get; }

    private static string GetMessage(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return error.Message;
    }
}
