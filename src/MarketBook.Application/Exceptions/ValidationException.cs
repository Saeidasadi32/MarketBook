// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Application.Exceptions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation.Results;

namespace MarketBook.Application.Exceptions;

/// <summary>
/// EN: Represents validation failures raised by the Application layer.
/// FA: خطاهای اعتبارسنجی ایجادشده در لایه Application را نمایش می‌دهد.
/// </summary>
public sealed class ApplicationValidationException : Exception
{
    private const string DefaultMessage =
        "One or more validation failures have occurred.";

    /// <summary>
    /// EN: Initializes an empty validation exception.
/// FA: استثناء اعتبارسنجی خالی را ایجاد می‌کند.
    /// </summary>
    public ApplicationValidationException()
        : base(DefaultMessage)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// EN: Initializes a validation exception with a message.
/// FA: استثناء اعتبارسنجی را با پیام ایجاد می‌کند.
    /// </summary>
    /// <param name="message">EN: Exception message. FA: پیام استثناء.</param>
    public ApplicationValidationException(string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// EN: Initializes a validation exception with a message and inner exception.
/// FA: استثناء اعتبارسنجی را با پیام و استثناء داخلی ایجاد می‌کند.
    /// </summary>
    /// <param name="message">EN: Exception message. FA: پیام استثناء.</param>
    /// <param name="innerException">EN: Inner exception. FA: استثناء داخلی.</param>
    public ApplicationValidationException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(innerException);

        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// EN: Initializes a validation exception from FluentValidation failures.
/// FA: استثناء اعتبارسنجی را از خطاهای FluentValidation ایجاد می‌کند.
    /// </summary>
    /// <param name="failures">EN: Validation failures. FA: خطاهای اعتبارسنجی.</param>
    public ApplicationValidationException(
        IEnumerable<ValidationFailure> failures)
        : base(DefaultMessage)
    {
        ArgumentNullException.ThrowIfNull(failures);

        Errors = failures
            .Where(static failure => failure is not null)
            .GroupBy(static failure => failure.PropertyName)
            .ToDictionary(
                static group => group.Key,
                static group => group
                    .Select(static failure => failure.ErrorMessage)
                    .ToArray());
    }

    /// <summary>
    /// EN: Gets validation errors grouped by property.
/// FA: خطاهای اعتبارسنجی گروه‌بندی‌شده بر اساس Property را دریافت می‌کند.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
