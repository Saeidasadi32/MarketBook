// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.TradingCalendars;
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById;

/// <summary>
/// EN: Handles retrieval of a trading calendar.
/// FA: دریافت تقویم معاملاتی را مدیریت می‌کند.
/// </summary>
public sealed class GetTradingCalendarByIdHandler
    : IRequestHandler<GetTradingCalendarByIdQuery, Result<GetTradingCalendarByIdResponse>>
{
    private readonly ITradingCalendarRepository _calendarRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetTradingCalendarByIdHandler(
        ITradingCalendarRepository calendarRepository)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        _calendarRepository = calendarRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetTradingCalendarByIdResponse>> Handle(
        GetTradingCalendarByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!TradingCalendarId.TryParse(
                request.Id,
                out TradingCalendarId? calendarId) ||
            calendarId is null)
        {
            return Result<GetTradingCalendarByIdResponse>.Fail(
                new Error("TradingCalendar.InvalidId", "The calendar identifier is invalid."));
        }

        TradingCalendar? calendar =
            await _calendarRepository.GetByIdAsync(calendarId, cancellationToken);

        if (calendar is null)
        {
            return Result<GetTradingCalendarByIdResponse>.Fail(
                new Error("TradingCalendar.NotFound", "The trading calendar was not found."));
        }

        TradingSessionContract[] sessions =
            calendar.Sessions
                .OrderBy(session => session.DayOfWeek)
                .Select(session => new TradingSessionContract(
                    (int)session.DayOfWeek,
                    session.OpensAt,
                    session.ClosesAt))
                .ToArray();

        CalendarDateExceptionContract[] exceptions =
            calendar.DateExceptions
                .OrderBy(item => item.Date)
                .Select(item => new CalendarDateExceptionContract(
                    item.Date,
                    item.IsHoliday,
                    item.IsHalfDay,
                    item.Description))
                .ToArray();

        GetTradingCalendarByIdResponse response = new(
            calendar.Id.Value.ToString(),
            calendar.MarketId.Value.ToString(),
            calendar.Year,
            (int)calendar.WeekendDays,
            calendar.IsActive,
            calendar.CreatedOn,
            sessions,
            exceptions);

        return Result<GetTradingCalendarByIdResponse>.Success(response);
    }
}
