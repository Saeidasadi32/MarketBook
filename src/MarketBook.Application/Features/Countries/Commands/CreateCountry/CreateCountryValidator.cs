// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.CreateCountry
// -----------------------------------------------------------------------------

using FluentValidation;

namespace MarketBook.Application.Features.Countries.Commands.CreateCountry;

/// <summary>
/// EN: Validates <see cref="CreateCountryCommand"/>.
/// FA: اعتبارسنجی فرمان ایجاد کشور.
/// </summary>
public sealed class CreateCountryValidator
    : AbstractValidator<CreateCountryCommand>
{
    /// <summary>
    /// EN: Initializes validator.
    /// FA: سازنده اعتبارسنجی.
    /// </summary>
    public CreateCountryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(2, 3);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(100);
    }
}
