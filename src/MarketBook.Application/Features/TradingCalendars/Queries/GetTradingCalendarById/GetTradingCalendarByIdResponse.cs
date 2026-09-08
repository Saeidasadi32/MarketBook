// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.TradingCalendars;

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById;

/// <summary>
/// EN: Represents full trading-calendar details.
/// FA: جزئیات کامل تقویم معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record GetTradingCalendarByIdResponse(
    string Id,
    string MarketId,
    int Year,
    int WeekendDays,
    bool IsActive,
    DateTimeOffset CreatedOn,
    IReadOnlyCollection<TradingSessionContract> Sessions,
    IReadOnlyCollection<CalendarDateExceptionContract> DateExceptions);
