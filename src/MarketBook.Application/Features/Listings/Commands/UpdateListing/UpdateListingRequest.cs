// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.UpdateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Listings.Commands.UpdateListing;

/// <summary>
/// EN: Represents a request to update mutable listing attributes.
/// FA: درخواست به‌روزرسانی ویژگی‌های قابل تغییر Listing را نشان می‌دهد.
/// </summary>
public sealed record UpdateListingRequest(
    string QuoteCurrencyId,
    string TradingSymbol,
    decimal TickSize,
    int PricePrecision);
