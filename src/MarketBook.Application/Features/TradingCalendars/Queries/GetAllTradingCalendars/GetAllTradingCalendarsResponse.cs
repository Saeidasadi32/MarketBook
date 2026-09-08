// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars;

/// <summary>
/// EN: Represents a paged trading-calendar response.
/// FA: پاسخ صفحه‌بندی‌شده تقویم‌های معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record GetAllTradingCalendarsResponse(
    IReadOnlyCollection<TradingCalendarListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents one calendar item in a paged result.
/// FA: یک آیتم تقویم را در نتیجه صفحه‌بندی‌شده نشان می‌دهد.
/// </summary>
public sealed record TradingCalendarListItemResponse(
    string Id,
    string MarketId,
    int Year,
    int WeekendDays,
    bool IsActive);
