// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay;

/// <summary>
/// EN: Represents the trading status and regular session for one date.
/// FA: وضعیت معاملاتی و Session عادی یک تاریخ را نشان می‌دهد.
/// </summary>
public sealed record EvaluateTradingDayResponse(
    DateOnly Date,
    bool IsTradingDay,
    bool IsHalfDay,
    TimeOnly? OpensAt,
    TimeOnly? ClosesAt);
