// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetListingById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Listings.Queries.GetListingById;

/// <summary>
/// EN: Represents listing details.
/// FA: جزئیات Listing را نشان می‌دهد.
/// </summary>
public sealed record GetListingByIdResponse(
    string Id,
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision,
    bool IsPrimary,
    DateTimeOffset CreatedOn,
    bool IsActive,
    DateTimeOffset? ActivatedOn,
    DateTimeOffset? DeactivatedOn);
