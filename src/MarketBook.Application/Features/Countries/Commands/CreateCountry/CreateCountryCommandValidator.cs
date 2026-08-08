// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Countries.Commands.CreateCountry
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation;

namespace MarketBook.Application.Features.Countries.Commands.CreateCountry;

/// <summary>
/// EN: Validates <see cref="CreateCountryCommand"/>.
/// FA: اعتبارسنجی <see cref="CreateCountryCommand"/>.
/// </summary>
public sealed class CreateCountryCommandValidator
    : AbstractValidator<CreateCountryCommand>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CreateCountryCommandValidator"/> class.
    /// FA: نمونه جدیدی از <see cref="CreateCountryCommandValidator"/> را ایجاد می‌کند.
    /// </summary>
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(2);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(100);
    }
}
