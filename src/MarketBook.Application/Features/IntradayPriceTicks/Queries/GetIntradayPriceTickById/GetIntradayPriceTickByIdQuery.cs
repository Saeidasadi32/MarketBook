// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById;

/// <summary>
/// EN: Query for retrieving an intraday price tick by identifier.
/// FA: پرس‌وجوی دریافت Tick قیمت درون‌روزی بر اساس شناسه.
/// </summary>
public sealed record GetIntradayPriceTickByIdQuery(string Id)
    : IRequest<Result<GetIntradayPriceTickByIdResponse>>;
