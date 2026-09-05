// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// Namespace : MarketBook.Api.Contracts.Venues
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Venues.Commands.CreateVenue;

/// <summary>
/// EN: Represents the HTTP request body used to create a trading venue.
/// FA: بدنه درخواست HTTP برای ایجاد یک بستر معاملاتی را نمایش می‌دهد.
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
public sealed record CreateVenueRequest(
    string MarketId,
    string Code,
    string Name,
    int Type);
