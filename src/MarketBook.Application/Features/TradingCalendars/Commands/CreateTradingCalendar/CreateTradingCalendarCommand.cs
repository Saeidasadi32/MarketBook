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
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar;

/// <summary>
/// EN: Represents a command to create a trading calendar.
/// FA: فرمان ایجاد تقویم معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record CreateTradingCalendarCommand(
    string MarketId,
    int Year,
    int WeekendDays,
    IReadOnlyCollection<TradingSessionContract> Sessions,
    IReadOnlyCollection<CalendarDateExceptionContract> DateExceptions)
    : IRequest<Result<TradingCalendarId>>;
