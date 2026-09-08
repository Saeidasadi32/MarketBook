// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.TradingCalendars;
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.Entities;
using MarketBook.Domain.Calendar.Enums;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar;

/// <summary>
/// EN: Replaces mutable trading-calendar configuration.
/// FA: تنظیمات قابل تغییر تقویم معاملاتی را جایگزین می‌کند.
/// </summary>
public sealed class UpdateTradingCalendarHandler
    : IRequestHandler<UpdateTradingCalendarCommand, Result<TradingCalendarId>>
{
    private readonly ITradingCalendarRepository _calendarRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public UpdateTradingCalendarHandler(
        ITradingCalendarRepository calendarRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _calendarRepository = calendarRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles replacement of weekend, sessions, holidays, and half-days.
    /// FA: جایگزینی آخرهفته، Sessionها، تعطیلات و نیمه‌روزها را پردازش می‌کند.
    /// </summary>
    public async Task<Result<TradingCalendarId>> Handle(
        UpdateTradingCalendarCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Sessions is null ||
            request.DateExceptions is null)
        {
            return Result<TradingCalendarId>.Fail(
                new Error(
                    "TradingCalendar.InvalidConfiguration",
                    "Sessions and date exceptions collections are required."));
        }

        if (!TradingCalendarId.TryParse(
                request.Id,
                out TradingCalendarId? calendarId) ||
            calendarId is null)
        {
            return Result<TradingCalendarId>.Fail(
                new Error("TradingCalendar.InvalidId", "The calendar identifier is invalid."));
        }

        TradingCalendar? calendar =
            await _calendarRepository.GetByIdAsync(calendarId, cancellationToken);

        if (calendar is null)
        {
            return Result<TradingCalendarId>.Fail(
                new Error("TradingCalendar.NotFound", "The trading calendar was not found."));
        }

        try
        {
            calendar.ChangeWeekendDays(
                (TradingWeekDays)request.WeekendDays);

            TradingSession[] sessions =
                request.Sessions
                    .Select(CreateSession)
                    .ToArray();

            calendar.ReplaceSessions(sessions);

            DateOnly[] existingDates =
                calendar.DateExceptions
                    .Select(item => item.Date)
                    .ToArray();

            foreach (DateOnly date in existingDates)
            {
                calendar.RemoveDateException(date);
            }

            foreach (CalendarDateExceptionContract exception in request.DateExceptions)
            {
                if (exception.IsHoliday == exception.IsHalfDay)
                {
                    return Result<TradingCalendarId>.Fail(
                        new Error(
                            "TradingCalendar.InvalidDateException",
                            "Each date exception must be either a holiday or a half-day."));
                }

                if (exception.IsHoliday)
                {
                    calendar.AddHoliday(exception.Date, exception.Description);
                }
                else
                {
                    calendar.AddHalfDay(exception.Date, exception.Description);
                }
            }
        }
        catch (ArgumentException exception)
        {
            return Result<TradingCalendarId>.Fail(
                new Error("TradingCalendar.InvalidConfiguration", exception.Message));
        }

        _calendarRepository.Update(calendar);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TradingCalendarId>.Success(calendar.Id);
    }

    private static TradingSession CreateSession(TradingSessionContract contract)
    {
        if (contract.DayOfWeek < 0 || contract.DayOfWeek > 6)
        {
            throw new ArgumentOutOfRangeException(
                nameof(contract.DayOfWeek),
                "DayOfWeek must be between 0 and 6.");
        }

        return new TradingSession(
            (DayOfWeek)contract.DayOfWeek,
            contract.OpensAt,
            contract.ClosesAt);
    }
}
