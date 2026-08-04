// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.Queries.GetCountryById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Countries.Queries.GetCountryById;

/// <summary>
/// EN: Handles <see cref="GetCountryByIdQuery"/>.
/// FA: پردازش‌کننده <see cref="GetCountryByIdQuery"/>.
/// </summary>
public sealed class GetCountryByIdQueryHandler
    : IQueryHandler<GetCountryByIdQuery, Result<CountryResponse>>
{
    /// <inheritdoc />
    public Task<Result<CountryResponse>> Handle(
        GetCountryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            Result<CountryResponse>.Fail(
                new Error(
                    "Countries.NotImplemented",
                    "Countries query has not been implemented yet.")));
    }
}
