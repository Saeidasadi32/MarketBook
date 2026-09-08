// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.Entities;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay;

/// <summary>
/// EN: Handles date evaluation using configurable weekends, holidays, and sessions.
/// FA: ارزیابی تاریخ را با آخرهفته قابل تنظیم، تعطیلات و Sessionها مدیریت می‌کند.
/// </summary>
public sealed class EvaluateTradingDayHandler
    : IRequestHandler<EvaluateTradingDayQuery, Result<EvaluateTradingDayResponse>>
{
    private readonly ITradingCalendarRepository _calendarRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public EvaluateTradingDayHandler(
        ITradingCalendarRepository calendarRepository)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        _calendarRepository = calendarRepository;
    }

    /// <summary>
    /// EN: Handles the date evaluation query.
    /// FA: پرس‌وجوی ارزیابی تاریخ را پردازش می‌کند.
    /// </summary>
    public async Task<Result<EvaluateTradingDayResponse>> Handle(
        EvaluateTradingDayQuery request,
        CancellationToken cancellationToken)
    {
        if (!TradingCalendarId.TryParse(
                request.Id,
                out TradingCalendarId? calendarId) ||
            calendarId is null)
        {
            return Result<EvaluateTradingDayResponse>.Fail(
                new Error("TradingCalendar.InvalidId", "The calendar identifier is invalid."));
        }

        TradingCalendar? calendar =
            await _calendarRepository.GetByIdAsync(calendarId, cancellationToken);

        if (calendar is null)
        {
            return Result<EvaluateTradingDayResponse>.Fail(
                new Error("TradingCalendar.NotFound", "The trading calendar was not found."));
        }

        bool isTradingDay = calendar.IsTradingDay(request.Date);
        bool isHalfDay = calendar.IsHalfDay(request.Date);
        TradingSession? session = calendar.GetSession(request.Date);

        EvaluateTradingDayResponse response = new(
            request.Date,
            isTradingDay,
            isHalfDay,
            session?.OpensAt,
            session?.ClosesAt);

        return Result<EvaluateTradingDayResponse>.Success(response);
    }
}
