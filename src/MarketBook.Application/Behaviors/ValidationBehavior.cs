// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Behaviors
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation;
using FluentValidation.Results;
using MarketBook.Application.Exceptions;
using MediatR;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Validates incoming requests before passing them to the next pipeline step.
/// FA: قبل از ارسال درخواست به مرحله بعد Pipeline، اعتبارسنجی را انجام می‌دهد.
/// </summary>
/// <typeparam name="TRequest">EN: Request type. FA: نوع درخواست.</typeparam>
/// <typeparam name="TResponse">EN: Response type. FA: نوع پاسخ.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>[] _validators;

    /// <summary>
    /// EN: Initializes a new instance of the validation behavior.
    /// FA: نمونه جدیدی از رفتار اعتبارسنجی را ایجاد می‌کند.
    /// </summary>
    /// <param name="validators">EN: Registered validators. FA: اعتبارسنجی‌کننده‌های ثبت‌شده.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        ArgumentNullException.ThrowIfNull(validators);

        _validators = validators.ToArray();
    }

    /// <summary>
    /// EN: Validates the request and invokes the next pipeline step when validation succeeds.
    /// FA: درخواست را اعتبارسنجی کرده و در صورت موفقیت مرحله بعدی Pipeline را اجرا می‌کند.
    /// </summary>
    /// <param name="request">EN: Request to validate. FA: درخواست مورد اعتبارسنجی.</param>
    /// <param name="next">EN: Next pipeline delegate. FA: مرحله بعد Pipeline.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Pipeline response. FA: پاسخ Pipeline.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        if (_validators.Length == 0)
        {
            return await next(cancellationToken);
        }

        ValidationContext<TRequest> context = new(request);

        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(
                validator => validator.ValidateAsync(
                    context,
                    cancellationToken)));

        List<ValidationFailure> failures = results
            .SelectMany(result => result.Errors)
            .Where(static failure => failure is not null)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ApplicationValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
