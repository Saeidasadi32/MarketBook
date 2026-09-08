// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.TradingCalendars;

namespace MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar;

/// <summary>
/// EN: Represents a request to replace mutable calendar configuration.
/// FA: درخواست جایگزینی تنظیمات قابل تغییر تقویم را نشان می‌دهد.
/// </summary>
public sealed record UpdateTradingCalendarRequest(
    int WeekendDays,
    IReadOnlyCollection<TradingSessionContract> Sessions,
    IReadOnlyCollection<CalendarDateExceptionContract> DateExceptions);
