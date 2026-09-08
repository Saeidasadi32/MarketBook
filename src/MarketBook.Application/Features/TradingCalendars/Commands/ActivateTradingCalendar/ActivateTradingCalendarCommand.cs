// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.ActivateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.ActivateTradingCalendar;

/// <summary>
/// EN: Represents a command to activate a trading calendar.
/// FA: فرمان فعال‌سازی تقویم معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record ActivateTradingCalendarCommand(
    string Id) : IRequest<Result<TradingCalendarId>>;
