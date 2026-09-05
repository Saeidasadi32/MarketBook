// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.CreateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Listings.Commands.CreateListing;

/// <summary>
/// EN: Represents a request to create a listing.
/// FA: درخواست ایجاد Listing را نشان می‌دهد.
/// </summary>
public sealed record CreateListingRequest(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision);
