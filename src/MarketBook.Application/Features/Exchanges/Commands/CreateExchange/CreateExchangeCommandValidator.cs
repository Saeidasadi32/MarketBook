// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.CreateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation;

namespace MarketBook.Application.Features.Exchanges.Commands.CreateExchange;

/// <summary>
/// EN: Validates <see cref="CreateExchangeCommand"/>.
/// FA: اعتبارسنجی <see cref="CreateExchangeCommand"/>.
/// </summary>
public sealed class CreateExchangeCommandValidator
    : AbstractValidator<CreateExchangeCommand>
{
    /// <summary>
    /// EN: Initializes a new instance of the validator.
    /// FA: نمونه جدیدی از اعتبارسنجی‌کننده را ایجاد می‌کند.
    /// </summary>
    public CreateExchangeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
