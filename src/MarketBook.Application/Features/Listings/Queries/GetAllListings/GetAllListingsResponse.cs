// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetAllListings
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Listings.Queries.GetAllListings;

/// <summary>
/// EN: Represents a paged listing response.
/// FA: پاسخ صفحه‌بندی‌شده Listingها را نشان می‌دهد.
/// </summary>
public sealed record GetAllListingsResponse(
    IReadOnlyCollection<ListingListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents a listing item in a paged collection.
/// FA: یک آیتم Listing در مجموعه صفحه‌بندی‌شده را نشان می‌دهد.
/// </summary>
public sealed record ListingListItemResponse(
    string Id,
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision,
    bool IsPrimary,
    bool IsActive);
