// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.DeactivateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.DeactivateTradingCalendar;

/// <summary>
/// EN: Represents a command to deactivate a trading calendar.
/// FA: فرمان غیرفعال‌سازی تقویم معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record DeactivateTradingCalendarCommand(
    string Id) : IRequest<Result<TradingCalendarId>>;
