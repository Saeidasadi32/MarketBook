// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById;
/// <summary>EN: Gets MarketPrice by id. FA: MarketPrice را با شناسه دریافت می‌کند.</summary>
public sealed record GetMarketPriceByIdQuery(string Id):IRequest<Result<GetMarketPriceByIdResponse>>;
