// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById;
/// <summary>EN: Represents MarketPrice details. FA: جزئیات MarketPrice را نمایش می‌دهد.</summary>
public sealed record GetMarketPriceByIdResponse(string Id,string ListingId,DateOnly TradingDate,decimal OpenPrice,decimal HighPrice,decimal LowPrice,decimal LastPrice,decimal ClosePrice,decimal PreviousClosePrice,decimal ReferencePrice,decimal LowerLimit,decimal UpperLimit,bool IsAtUpperLimit,bool IsAtLowerLimit,DateTimeOffset CreatedOn,DateTimeOffset UpdatedOn);
