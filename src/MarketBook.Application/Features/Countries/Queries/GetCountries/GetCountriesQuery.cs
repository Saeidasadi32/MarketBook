// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.Queries.GetCountries
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Countries.Queries.GetCountries;

/// <summary>
/// EN: Represents a query for retrieving countries with pagination.
/// FA: پرس‌وجوی دریافت کشورهای صفحه‌بندی‌شده.
/// </summary>
public sealed record GetCountriesQuery(
    int Page = PageRequest.DefaultPage,
    int PageSize = PageRequest.DefaultPageSize)
    : IQuery<Result<PagedResult<CountryResponse>>>;
