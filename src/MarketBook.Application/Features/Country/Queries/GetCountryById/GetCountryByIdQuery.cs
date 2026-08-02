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
/// EN: Represents a query for retrieving a country by its identifier.
/// FA: درخواست دریافت یک کشور بر اساس شناسه.
/// </summary>
/// <param name="CountryId">
/// EN: Country identifier.
/// FA: شناسه کشور.
/// </param>
public sealed record GetCountryByIdQuery(
    string CountryId)
    : IQuery<Result<CountryResponse>>;