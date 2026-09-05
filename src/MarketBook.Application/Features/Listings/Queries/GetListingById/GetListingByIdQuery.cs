// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetListingById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Listings.Queries.GetListingById;

/// <summary>
/// EN: Represents a query to retrieve a listing by identifier.
/// FA: پرس‌وجوی دریافت Listing بر اساس شناسه را نشان می‌دهد.
/// </summary>
public sealed record GetListingByIdQuery(
    string Id) : IRequest<Result<GetListingByIdResponse>>;
