// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.CreateCountry
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Application.Features.Countries.Commands.CreateCountry;

/// <summary>
/// EN: Handles country creation.
/// FA: پردازش ایجاد کشور.
/// </summary>
public sealed class CreateCountryCommandHandler
    : ICommandHandler<CreateCountryCommand, Result>
{
    private readonly ICountryRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes handler.
    /// FA: سازنده Handler.
    /// </summary>
    public CreateCountryCommandHandler(
        ICountryRepository repository,
        IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<Result> HandleAsync(
        CreateCountryCommand command,
        CancellationToken cancellationToken)
    {
        var code = new CountryCode(command.Code);

        if (await _repository.ExistsAsync(code, cancellationToken))
        {
            return Result.Failure(
                new Error(
                    "Country.AlreadyExists",
                    "Country already exists."));
        }

        var country = new Country(
            CountryId.New(),
            code,
            command.Name,
            new TimeZoneId(command.TimeZone));

        await _repository.AddAsync(
            country,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
