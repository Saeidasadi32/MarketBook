// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.MakePrimaryListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.MakePrimaryListing;

/// <summary>
/// EN: Represents a command to make a listing primary.
/// FA: فرمان تعیین Listing به‌عنوان پذیرش اصلی را نشان می‌دهد.
/// </summary>
public sealed record MakePrimaryListingCommand(
    string Id) : IRequest<Result<ListingId>>;
