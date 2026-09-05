// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.CreateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.CreateVenue;

/// <summary>
/// EN: Represents the command used to create a new trading venue.
/// FA: فرمان ایجاد یک بستر معاملاتی جدید را نمایش می‌دهد.
/// </summary>
/// <param name="MarketId">
/// EN: Identifier of the market to which the venue belongs.
/// FA: شناسه بازاری که بستر معاملاتی به آن تعلق دارد.
/// </param>
/// <param name="Code">
/// EN: Unique business code of the venue.
/// FA: کد تجاری یکتای بستر معاملاتی.
/// </param>
/// <param name="Name">
/// EN: Display name of the venue.
/// FA: نام نمایشی بستر معاملاتی.
/// </param>
/// <param name="Type">
/// EN: Trading venue type.
/// FA: نوع بستر معاملاتی.
/// </param>
public sealed record CreateVenueCommand(
    string MarketId,
    string Code,
    string Name,
    int Type) : IRequest<Result<VenueId>>;
