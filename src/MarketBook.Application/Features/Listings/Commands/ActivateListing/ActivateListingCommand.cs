// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.ActivateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.ActivateListing;

/// <summary>
/// EN: Represents a command to activate a listing.
/// FA: فرمان فعال‌سازی Listing را نشان می‌دهد.
/// </summary>
public sealed record ActivateListingCommand(
    string Id) : IRequest<Result<ListingId>>;
