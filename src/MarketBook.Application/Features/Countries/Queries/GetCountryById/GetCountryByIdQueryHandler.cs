// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Countries.Queries.GetCountryById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Countries.Mappings;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;

namespace MarketBook.Application.Features.Countries.Queries.GetCountryById;

/// <summary>
/// EN: Handles <see cref="GetCountryByIdQuery"/>.
/// FA: پردازش <see cref="GetCountryByIdQuery"/>.
/// </summary>
public sealed class GetCountryByIdQueryHandler
    : IQueryHandler<GetCountryByIdQuery, Result<CountryResponse>>
{
    private readonly ICountryRepository _repository;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="GetCountryByIdQueryHandler"/> class.
    /// FA: نمونه جدیدی از <see cref="GetCountryByIdQueryHandler"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="repository">Country repository.</param>
    public GetCountryByIdQueryHandler(ICountryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Gets a country by its identifier.
    /// FA: کشور را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="request">Country lookup request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The country response or a failure result.</returns>
    public async Task<Result<CountryResponse>> Handle(
        GetCountryByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        CountryId id = CountryId.Parse(request.Id);
        Country? country = await _repository.GetByIdAsync(id, cancellationToken);

        if (country is null)
        {
            return Result<CountryResponse>.Fail(
                new Error(
                    "Country.NotFound",
                    "Country was not found."));
        }

        return Result<CountryResponse>.Success(country.ToResponse());
    }
}
