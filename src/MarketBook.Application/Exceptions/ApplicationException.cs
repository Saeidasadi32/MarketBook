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
    public ApplicationException()
    {
        Error = new Error(
            "Application.Error",
            "An application error occurred.");
    }

    /// <summary>
    /// EN: Initializes a new instance with a message.
/// FA: نمونه جدید را با پیام ایجاد می‌کند.
    /// </summary>
    /// <param name="message">EN: Exception message. FA: پیام استثناء.</param>
    public ApplicationException(string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Error = new Error("Application.Error", message);
    }

    /// <summary>
    /// EN: Initializes a new instance with a message and inner exception.
/// FA: نمونه جدید را با پیام و استثناء داخلی ایجاد می‌کند.
    /// </summary>
    /// <param name="message">EN: Exception message. FA: پیام استثناء.</param>
    /// <param name="innerException">EN: Inner exception. FA: استثناء داخلی.</param>
    public ApplicationException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(innerException);

        Error = new Error("Application.Error", message);
    }

    /// <summary>
    /// EN: Initializes a new instance from a domain error.
    /// FA: نمونه جدید را از خطای دامنه ایجاد می‌کند.
    /// </summary>
    /// <param name="error">EN: Application error. FA: خطای برنامه.</param>
    public ApplicationException(Error error)
        : base(GetMessage(error))
    {
        Error = error;
    }

    /// <summary>
    /// EN: Initializes a new instance from an error and inner exception.
    /// FA: نمونه جدید را از خطا و استثناء داخلی ایجاد می‌کند.
    /// </summary>
    /// <param name="error">EN: Application error. FA: خطای برنامه.</param>
    /// <param name="innerException">EN: Inner exception. FA: استثناء داخلی.</param>
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

    private static string GetMessage(Error? error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return error.Message;
    }
}
