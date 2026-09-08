// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.ValueObjects; using MediatR;
namespace MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice;
/// <summary>EN: Creates a MarketPrice record. FA: رکورد MarketPrice ایجاد می‌کند.</summary>
public sealed record CreateMarketPriceCommand(string ListingId,DateOnly TradingDate,decimal OpenPrice,decimal HighPrice,decimal LowPrice,decimal LastPrice,decimal ClosePrice,decimal PreviousClosePrice,decimal ReferencePrice,decimal LowerLimit,decimal UpperLimit) : IRequest<Result<MarketPriceId>>;
