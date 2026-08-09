// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Countries.Commands.CreateCountry
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;

namespace MarketBook.Application.Features.Countries.Commands.CreateCountry;

/// <summary>
/// EN: Handles country creation.
/// FA: پردازش ایجاد کشور.
/// </summary>
public sealed class CreateCountryCommandHandler
    : ICommandHandler<CreateCountryCommand, Result<CountryId>>
{
    private readonly ICountryRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CreateCountryCommandHandler"/> class.
    /// FA: نمونه جدیدی از <see cref="CreateCountryCommandHandler"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="repository">Country repository.</param>
    /// <param name="dbContext">Application database context.</param>
    public CreateCountryCommandHandler(
        ICountryRepository repository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _repository = repository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Creates a new country.
    /// FA: یک کشور جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="request">Country creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    public async Task<Result<CountryId>> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        CountryCode code = new(request.Code);

        if (await _repository.ExistsAsync(code, cancellationToken))
        {
            return Result<CountryId>.Fail(
                new Error(
                    "Country.AlreadyExists",
                    "Country already exists."));
        }

        CountryId countryId = CountryId.New();

        Country country = new(
            CountryId.New(),
            code,
            request.Name,
            new TimeZoneId(request.TimeZone));

        await _repository.AddAsync(country, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CountryId>.Success(countryId);
    }
}
