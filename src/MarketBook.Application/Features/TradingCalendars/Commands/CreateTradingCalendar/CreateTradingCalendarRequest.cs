// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.TradingCalendars;

namespace MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar;

/// <summary>
/// EN: Represents a request to create a yearly trading calendar.
/// FA: درخواست ایجاد تقویم معاملاتی سالانه را نشان می‌دهد.
/// </summary>
public sealed record CreateTradingCalendarRequest(
    string MarketId,
    int Year,
    int WeekendDays,
    IReadOnlyCollection<TradingSessionContract> Sessions,
    IReadOnlyCollection<CalendarDateExceptionContract> DateExceptions);
