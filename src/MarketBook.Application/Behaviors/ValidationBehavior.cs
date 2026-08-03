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
using MediatR;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Validates incoming requests before passing them to the next pipeline step.
/// FA: قبل از ارسال درخواست به مرحله بعد Pipeline، اعتبارسنجی را انجام می‌دهد.
/// </summary>
/// <typeparam name="TRequest">
/// EN: Request type.
/// FA: نوع درخواست.
/// </typeparam>
/// <typeparam name="TResponse">
/// EN: Response type.
/// FA: نوع پاسخ.
/// </typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// EN: Initializes a new instance of the ValidationBehavior class.
    /// FA: نمونه جدیدی از کلاس ValidationBehavior را ایجاد می‌کند.
    /// </summary>
    /// <param name="validators">
    /// EN: Registered validators.
    /// FA: اعتبارسنج‌های ثبت شده.
    /// </param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var errors = failures
            .SelectMany(v => v.Errors)
            .Where(e => e is not null)
            .ToList();

        if (errors.Count == 0)
            return await next();

        throw new ValidationException(errors);
    }
}