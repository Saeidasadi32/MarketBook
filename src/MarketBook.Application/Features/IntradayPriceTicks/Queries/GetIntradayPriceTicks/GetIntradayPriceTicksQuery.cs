// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks;

/// <summary>
/// EN: Query for paged intraday ticks of one Listing and trading date.
/// FA: پرس‌وجوی صفحه‌بندی Tickهای یک Listing و تاریخ معاملاتی.
/// </summary>
public sealed record GetIntradayPriceTicksQuery(
    string ListingId,
    DateOnly TradingDate,
    int Page,
    int PageSize)
    : IRequest<Result<GetIntradayPriceTicksResponse>>;
