// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice;
/// <summary>EN: Represents create request. FA: درخواست ایجاد قیمت بازار را نمایش می‌دهد.</summary>
public sealed record CreateMarketPriceRequest(string ListingId,DateOnly TradingDate,decimal OpenPrice,decimal HighPrice,decimal LowPrice,decimal LastPrice,decimal ClosePrice,decimal PreviousClosePrice,decimal ReferencePrice,decimal LowerLimit,decimal UpperLimit);
