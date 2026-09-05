// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetVenueById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Venues.Queries.GetVenueById;

/// <summary>
/// EN: Represents the query used to retrieve a trading venue by its identifier.
/// FA: پرس‌وجوی دریافت یک بستر معاملاتی بر اساس شناسه آن را نمایش می‌دهد.
/// </summary>
/// <param name="Id">
/// EN: Venue identifier.
/// FA: شناسه بستر معاملاتی.
/// </param>
public sealed record GetVenueByIdQuery(
    string Id) : IRequest<Result<GetVenueByIdResponse>>;
