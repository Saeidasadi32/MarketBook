// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick;

/// <summary>
/// EN: API request for creating an intraday price tick.
/// FA: درخواست API برای ایجاد Tick قیمت درون‌روزی.
/// </summary>
public sealed record CreateIntradayPriceTickRequest(
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset OccurredAt,
    long SequenceNumber,
    decimal Price,
    long Volume);
