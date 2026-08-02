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
using MarketBook.Domain.Common;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Validates requests before they are handled.
/// FA: قبل از اجرای درخواست، اعتبارسنجی را انجام می‌دهد.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// EN: Initializes a new instance of the
    /// <see cref="ValidationBehavior{TRequest,TResponse}"/> class.
    /// FA: نمونه جدیدی از کلاس
    /// <see cref="ValidationBehavior{TRequest,TResponse}"/>
    /// را ایجاد می‌کند.
    /// </summary>
    /// <param name="validators">
    /// EN: Validators for the request.
    /// FA: اعتبارسنج‌های مربوط به درخواست.
    /// </param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// EN: Validates the request before invoking the next handler.
    /// FA: قبل از اجرای Handler بعدی، درخواست را اعتبارسنجی می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Incoming request.
    /// FA: درخواست ورودی.
    /// </param>
    /// <param name="next">
    /// EN: Delegate representing the next step in the pipeline.
    /// FA: نماینده مرحله بعدی Pipeline.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Handler response.
    /// FA: پاسخ Handler.
    /// </returns>
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
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToArray();

        if (errors.Length == 0)
            return await next();

        if (typeof(TResponse) == typeof(Result))
        {
            var error = new Error(
                "Validation.Error",
                string.Join(Environment.NewLine,
                    errors.Select(e => e.ErrorMessage)));

            return (TResponse)(object)Result.Failure(error);
        }

        throw new ValidationException(errors);
    }
}