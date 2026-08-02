// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Country.Queries.GetCountryById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Country.Queries.GetCountryById;

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
            Result<CountryResponse>.Failure(
                new Error(
                    "Country.NotImplemented",
                    "Country query has not been implemented yet.")));
    }
}