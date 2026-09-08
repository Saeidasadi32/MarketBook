// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById;

/// <summary>
/// EN: Response for a single intraday price tick.
/// FA: پاسخ یک Tick قیمت درون‌روزی.
/// </summary>
public sealed record GetIntradayPriceTickByIdResponse(
    string Id,
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset OccurredAt,
    long SequenceNumber,
    decimal Price,
    long Volume,
    decimal TradeValue,
    DateTimeOffset CreatedOn);
