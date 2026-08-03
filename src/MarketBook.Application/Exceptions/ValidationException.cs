// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Exceptions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation.Results;

namespace MarketBook.Application.Exceptions;

/// <summary>
/// EN: Represents a validation exception.
/// FA: استثنای اعتبارسنجی را نمایش می‌دهد.
/// </summary>
public sealed class ApplicationValidationException : Exception
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ApplicationValidationException"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ApplicationValidationException"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="failures">
    /// EN: Validation failures.
    /// FA: خطاهای اعتبارسنجی.
    /// </param>
    public ApplicationValidationException(
        IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }

    /// <summary>
    /// EN: Gets validation errors.
    /// FA: خطاهای اعتبارسنجی را دریافت می‌کند.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}