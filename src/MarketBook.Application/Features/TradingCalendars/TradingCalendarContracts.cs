// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.TradingCalendars;

/// <summary>
/// EN: Represents one regular weekly trading session in API/Application contracts.
/// FA: یک Session معاملاتی عادی هفتگی را در قراردادهای Application/API نمایش می‌دهد.
/// </summary>
public sealed record TradingSessionContract(
    int DayOfWeek,
    TimeOnly OpensAt,
    TimeOnly ClosesAt);

/// <summary>
/// EN: Represents a date-specific holiday or half-day.
/// FA: یک تعطیلی یا نیمه‌روز تاریخ‌محور را نمایش می‌دهد.
/// </summary>
public sealed record CalendarDateExceptionContract(
    DateOnly Date,
    bool IsHoliday,
    bool IsHalfDay,
    string? Description);
