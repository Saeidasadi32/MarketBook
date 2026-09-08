// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Commands.UpdateMarketPrice
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.ValueObjects; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Commands.UpdateMarketPrice;
/// <summary>EN: Updates a MarketPrice record. FA: رکورد MarketPrice را به‌روزرسانی می‌کند.</summary>
public sealed record UpdateMarketPriceCommand(string Id,decimal OpenPrice,decimal HighPrice,decimal LowPrice,decimal LastPrice,decimal ClosePrice,decimal PreviousClosePrice,decimal ReferencePrice,decimal LowerLimit,decimal UpperLimit):IRequest<Result<MarketPriceId>>;
