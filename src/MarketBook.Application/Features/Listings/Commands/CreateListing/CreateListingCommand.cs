// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.CreateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.CreateListing;

/// <summary>
/// EN: Represents a command to create a listing.
/// FA: فرمان ایجاد Listing را نشان می‌دهد.
/// </summary>
public sealed record CreateListingCommand(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision) : IRequest<Result<ListingId>>;
