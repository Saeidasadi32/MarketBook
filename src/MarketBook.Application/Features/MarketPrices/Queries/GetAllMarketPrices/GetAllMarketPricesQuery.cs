// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices;
/// <summary>EN: Gets paged MarketPrices. FA: MarketPriceها را صفحه‌بندی‌شده دریافت می‌کند.</summary>
public sealed record GetAllMarketPricesQuery(int Page,int PageSize):IRequest<Result<GetAllMarketPricesResponse>>;
