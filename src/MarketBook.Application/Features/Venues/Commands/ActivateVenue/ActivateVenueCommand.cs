// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.ActivateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.ActivateVenue;

/// <summary>
/// EN: Represents a command to activate an existing venue.
/// FA: فرمان فعال‌سازی یک محل معاملاتی موجود را نشان می‌دهد.
/// </summary>
public sealed record ActivateVenueCommand(
    string Id) : IRequest<Result<VenueId>>;
