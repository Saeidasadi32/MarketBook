// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.UpdateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.UpdateVenue;

/// <summary>
/// Represents a command for updating a venue.
/// <para>
/// فرمان به‌روزرسانی یک محل معاملاتی را نشان می‌دهد.
/// </para>
/// </summary>
public sealed record UpdateVenueCommand(
    string Id,
    string Name,
    int Type) : IRequest<Result<VenueId>>;
