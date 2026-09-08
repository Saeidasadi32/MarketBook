// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar
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
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar;

/// <summary>
/// EN: Handles creation of yearly market trading calendars.
/// FA: ایجاد تقویم معاملاتی سالانه بازار را مدیریت می‌کند.
/// </summary>
public sealed class CreateTradingCalendarHandler
    : IRequestHandler<CreateTradingCalendarCommand, Result<TradingCalendarId>>
{
    private readonly ITradingCalendarRepository _calendarRepository;
    private readonly IMarketRepository _marketRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreateTradingCalendarHandler(
        ITradingCalendarRepository calendarRepository,
        IMarketRepository marketRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        ArgumentNullException.ThrowIfNull(marketRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _calendarRepository = calendarRepository;
        _marketRepository = marketRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles calendar creation.
    /// FA: ایجاد تقویم را پردازش می‌کند.
    /// </summary>
    public async Task<Result<TradingCalendarId>> Handle(
        CreateTradingCalendarCommand request,
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

        if (!MarketId.TryParse(request.MarketId, out MarketId? marketId) ||
            marketId is null)
        {
            return Fail("TradingCalendar.InvalidMarketId", "The market identifier is invalid.");
        }

        Market? market =
            await _marketRepository.GetByIdAsync(marketId, cancellationToken);

        if (market is null)
        {
            return Fail("TradingCalendar.MarketNotFound", "The specified market was not found.");
        }

        if (!market.IsActive)
        {
            return Fail("TradingCalendar.MarketInactive", "The specified market is inactive.");
        }

        if (await _calendarRepository.ExistsAsync(
                marketId,
                request.Year,
                cancellationToken))
        {
            return Fail(
                "TradingCalendar.DuplicateMarketYear",
                "A trading calendar already exists for the specified market and year.");
        }

        Result<TradingCalendar> buildResult = BuildCalendar(request, marketId);

        if (!buildResult.IsSuccess)
        {
            return Result<TradingCalendarId>.Fail(buildResult.Error);
        }

        TradingCalendar calendar = buildResult.Value!;

        await _calendarRepository.AddAsync(calendar, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TradingCalendarId>.Success(calendar.Id);
    }

    private static Result<TradingCalendar> BuildCalendar(
        CreateTradingCalendarCommand request,
        MarketId marketId)
    {
        try
        {
            TradingCalendar calendar = TradingCalendar.Create(
                marketId,
                request.Year,
                (TradingWeekDays)request.WeekendDays);

            TradingSession[] sessions =
                request.Sessions
                    .Select(CreateSession)
                    .ToArray();

            calendar.ReplaceSessions(sessions);

            foreach (CalendarDateExceptionContract exception in request.DateExceptions)
            {
                if (exception.IsHoliday == exception.IsHalfDay)
                {
                    return Result<TradingCalendar>.Fail(
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

            return Result<TradingCalendar>.Success(calendar);
        }
        catch (ArgumentException exception)
        {
            return Result<TradingCalendar>.Fail(
                new Error("TradingCalendar.InvalidConfiguration", exception.Message));
        }
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

    private static Result<TradingCalendarId> Fail(
        string code,
        string message)
        => Result<TradingCalendarId>.Fail(new Error(code, message));
}
