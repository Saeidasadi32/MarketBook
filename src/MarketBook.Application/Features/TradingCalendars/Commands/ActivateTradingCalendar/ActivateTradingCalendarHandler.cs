// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Commands.ActivateTradingCalendar
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Commands.ActivateTradingCalendar;

/// <summary>
/// EN: Handles activate trading calendars.
/// FA: فعال‌سازی تقویم معاملاتی را مدیریت می‌کند.
/// </summary>
public sealed class ActivateTradingCalendarHandler
    : IRequestHandler<ActivateTradingCalendarCommand, Result<TradingCalendarId>>
{
    private readonly ITradingCalendarRepository _calendarRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public ActivateTradingCalendarHandler(
        ITradingCalendarRepository calendarRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _calendarRepository = calendarRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the command.
    /// FA: فرمان را پردازش می‌کند.
    /// </summary>
    public async Task<Result<TradingCalendarId>> Handle(
        ActivateTradingCalendarCommand request,
        CancellationToken cancellationToken)
    {
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

        calendar.Activate();
        _calendarRepository.Update(calendar);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TradingCalendarId>.Success(calendar.Id);
    }
}
