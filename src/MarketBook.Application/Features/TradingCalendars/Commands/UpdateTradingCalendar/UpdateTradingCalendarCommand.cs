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
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar;

/// <summary>
/// EN: Represents a command to update calendar configuration.
/// FA: فرمان به‌روزرسانی تنظیمات تقویم را نشان می‌دهد.
/// </summary>
public sealed record UpdateTradingCalendarCommand(
    string Id,
    int WeekendDays,
    IReadOnlyCollection<TradingSessionContract> Sessions,
    IReadOnlyCollection<CalendarDateExceptionContract> DateExceptions)
    : IRequest<Result<TradingCalendarId>>;
