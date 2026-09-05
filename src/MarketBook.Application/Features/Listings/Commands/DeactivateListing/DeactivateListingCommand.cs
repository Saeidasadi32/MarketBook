// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.DeactivateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.DeactivateListing;

/// <summary>
/// EN: Represents a command to deactivate a listing.
/// FA: فرمان غیرفعال‌سازی Listing را نشان می‌دهد.
/// </summary>
public sealed record DeactivateListingCommand(
    string Id) : IRequest<Result<ListingId>>;
