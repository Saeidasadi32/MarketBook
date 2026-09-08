// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById;

/// <summary>
/// EN: Represents a query to retrieve one trading calendar.
/// FA: پرس‌وجوی دریافت یک تقویم معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record GetTradingCalendarByIdQuery(
    string Id) : IRequest<Result<GetTradingCalendarByIdResponse>>;
