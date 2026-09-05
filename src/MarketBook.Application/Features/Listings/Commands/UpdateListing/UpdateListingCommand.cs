// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.UpdateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.UpdateListing;

/// <summary>
/// EN: Represents a command to update a listing.
/// FA: فرمان به‌روزرسانی Listing را نشان می‌دهد.
/// </summary>
public sealed record UpdateListingCommand(
    string Id,
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision) : IRequest<Result<ListingId>>;
