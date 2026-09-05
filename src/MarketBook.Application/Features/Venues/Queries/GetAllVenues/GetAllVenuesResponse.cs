// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetAllVenues
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Venues.Queries.GetAllVenues;

/// <summary>
/// EN: Represents the paged response containing trading venues.
/// FA: پاسخ صفحه‌بندی‌شده شامل بسترهای معاملاتی را نمایش می‌دهد.
/// </summary>
/// <param name="Items">
/// EN: Trading venues included in the requested page.
/// FA: بسترهای معاملاتی موجود در صفحه مورد درخواست.
/// </param>
/// <param name="Page">
/// EN: Normalized page number.
/// FA: شماره صفحه نرمال‌شده.
/// </param>
/// <param name="PageSize">
/// EN: Normalized page size.
/// FA: اندازه صفحه نرمال‌شده.
/// </param>
/// <param name="TotalCount">
/// EN: Total number of trading venues.
/// FA: تعداد کل بسترهای معاملاتی.
/// </param>
/// <param name="TotalPages">
/// EN: Total number of available pages.
/// FA: تعداد کل صفحات موجود.
/// </param>
public sealed record GetAllVenuesResponse(
    IReadOnlyCollection<VenueListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents a trading venue item in a paged list.
/// FA: یک مورد بستر معاملاتی را در فهرست صفحه‌بندی‌شده نمایش می‌دهد.
/// </summary>
/// <param name="Id">
/// EN: Venue identifier.
/// FA: شناسه بستر معاملاتی.
/// </param>
/// <param name="MarketId">
/// EN: Associated market identifier.
/// FA: شناسه بازار مرتبط.
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
public sealed record VenueListItemResponse(
    string Id,
    string MarketId,
    string Code,
    string Name,
    int Type,
    DateTimeOffset CreatedOn,
    bool IsActive);
