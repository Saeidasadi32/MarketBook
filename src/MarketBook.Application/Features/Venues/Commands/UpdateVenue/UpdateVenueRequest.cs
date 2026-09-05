// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.UpdateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Venues.Commands.UpdateVenue;

/// <summary>
/// Represents the request used to update a venue.
/// <para>
/// درخواست مورد استفاده برای به‌روزرسانی محل معاملاتی را نشان می‌دهد.
/// </para>
/// </summary>
public sealed record UpdateVenueRequest(
    string Name,
    int Type);
