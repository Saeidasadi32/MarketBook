// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.Queries.GetCountryById
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Countries.Queries.GetCountryById;

/// <summary>
/// EN: Represents a query for retrieving a country by identifier.
/// FA: پرس‌وجوی دریافت کشور بر اساس شناسه.
/// </summary>
/// <param name="Id">
/// EN: Country identifier.
/// FA: شناسه کشور.
/// </param>
public sealed record GetCountryByIdQuery(
    string Id)
    : IQuery<Result<CountryResponse>>;
