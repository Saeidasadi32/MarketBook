// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Countries.Queries.GetCountries
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Countries.Mappings;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Countries.Queries.GetCountries;

/// <summary>
/// EN: Handles <see cref="GetCountriesQuery"/>.
/// FA: پرس‌وجوی <see cref="GetCountriesQuery"/> را پردازش می‌کند.
/// </summary>
public sealed class GetCountriesQueryHandler
    : IQueryHandler<
        GetCountriesQuery,
        Result<PagedResult<CountryResponse>>>
{
    private readonly ICountryRepository _repository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repository"></param>
    public GetCountriesQueryHandler(
        ICountryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<PagedResult<CountryResponse>>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Domain.Country.Aggregates.Country> result =
            await _repository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        IReadOnlyList<CountryResponse> items =
            result.Items
                .Select(country => country.ToResponse())
                .ToList();

        PagedResult<CountryResponse> response =
            new(
                items,
                result.Page,
                result.PageSize,
                result.TotalCount);

        return Result<PagedResult<CountryResponse>>.Success(
            response);
    }
}
