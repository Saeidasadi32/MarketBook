// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetVenueById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Venues.Queries.GetVenueById;

/// <summary>
/// EN: Represents the response returned when a trading venue is retrieved by its identifier.
/// FA: پاسخ بازگردانده‌شده هنگام دریافت یک بستر معاملاتی بر اساس شناسه آن را نمایش می‌دهد.
/// </summary>
/// <param name="Id">
/// EN: Venue identifier.
/// FA: شناسه بستر معاملاتی.
/// </param>
/// <param name="MarketId">
/// EN: Identifier of the market associated with the venue.
/// FA: شناسه بازاری که بستر معاملاتی به آن مرتبط است.
/// </param>
/// <param name="Code">
/// EN: Venue business code.
/// FA: کد تجاری بستر معاملاتی.
/// </param>
/// <param name="Name">
/// EN: Venue display name.
/// FA: نام نمایشی بستر معاملاتی.
/// </param>
/// <param name="Type">
/// EN: Trading venue type.
/// FA: نوع بستر معاملاتی.
/// </param>
/// <param name="CreatedOn">
/// EN: UTC timestamp when the venue was created.
/// FA: زمان ایجاد بستر معاملاتی بر مبنای UTC.
/// </param>
/// <param name="IsActive">
/// EN: Indicates whether the venue is active.
/// FA: مشخص می‌کند آیا بستر معاملاتی فعال است یا خیر.
/// </param>
public sealed record GetVenueByIdResponse(
    string Id,
    string MarketId,
    string Code,
    string Name,
    int Type,
    DateTimeOffset CreatedOn,
    bool IsActive);
