// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.DeactivateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.DeactivateVenue;

/// <summary>
/// EN: Represents a command to deactivate an existing venue.
/// FA: فرمان غیرفعال‌سازی یک محل معاملاتی موجود را نشان می‌دهد.
/// </summary>
public sealed record DeactivateVenueCommand(
    string Id) : IRequest<Result<VenueId>>;
