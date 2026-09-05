// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetAllListings
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Listings.Queries.GetAllListings;

/// <summary>
/// EN: Represents a paged listing query.
/// FA: پرس‌وجوی صفحه‌بندی‌شده Listingها را نشان می‌دهد.
/// </summary>
public sealed record GetAllListingsQuery(
    int Page = 1,
    int PageSize = 20) : IRequest<Result<GetAllListingsResponse>>;
