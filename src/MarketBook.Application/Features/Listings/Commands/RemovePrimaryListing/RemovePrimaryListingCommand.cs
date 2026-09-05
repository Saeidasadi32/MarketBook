// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.RemovePrimaryListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.RemovePrimaryListing;

/// <summary>
/// EN: Represents a command to removeprimary a listing.
/// FA: فرمان حذف وضعیت اصلی Listing را نشان می‌دهد.
/// </summary>
public sealed record RemovePrimaryListingCommand(
    string Id) : IRequest<Result<ListingId>>;
