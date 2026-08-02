// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Country.Commands.CreateCountry
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Country.Commands.CreateCountry;

/// <summary>
/// EN: Handles <see cref="CreateCountryCommand"/>.
/// FA: پردازش‌کننده <see cref="CreateCountryCommand"/>.
/// </summary>
public sealed class CreateCountryCommandHandler
    : ICommandHandler<CreateCountryCommand, Result>
{
    /// <summary>
    /// EN: Handles the command.
    /// FA: فرمان را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Incoming command.
    /// FA: فرمان ورودی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: Operation result.
    /// FA: نتیجه عملیات.
    /// </returns>
    public Task<Result> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken)
    {
        // Domain logic will be added after Infrastructure is implemented.
        return Task.FromResult(Result.Success());
    }
}