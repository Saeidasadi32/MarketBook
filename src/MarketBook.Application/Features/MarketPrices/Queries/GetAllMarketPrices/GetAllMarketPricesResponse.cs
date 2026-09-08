// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices;
/// <summary>EN: Represents paged MarketPrices. FA: پاسخ صفحه‌بندی‌شده MarketPriceها را نمایش می‌دهد.</summary>
public sealed record GetAllMarketPricesResponse(IReadOnlyCollection<MarketPriceListItemResponse> Items,int Page,int PageSize,int TotalCount,int TotalPages);
/// <summary>EN: Represents a MarketPrice list item. FA: آیتم فهرست MarketPrice را نمایش می‌دهد.</summary>
public sealed record MarketPriceListItemResponse(string Id,string ListingId,DateOnly TradingDate,decimal LastPrice,decimal ClosePrice,decimal LowerLimit,decimal UpperLimit);
