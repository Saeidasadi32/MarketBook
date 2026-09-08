// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars;

/// <summary>
/// EN: Handles paged trading-calendar retrieval.
/// FA: دریافت صفحه‌بندی‌شده تقویم‌های معاملاتی را مدیریت می‌کند.
/// </summary>
public sealed class GetAllTradingCalendarsHandler
    : IRequestHandler<GetAllTradingCalendarsQuery, Result<GetAllTradingCalendarsResponse>>
{
    private readonly ITradingCalendarRepository _calendarRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetAllTradingCalendarsHandler(
        ITradingCalendarRepository calendarRepository)
    {
        ArgumentNullException.ThrowIfNull(calendarRepository);
        _calendarRepository = calendarRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetAllTradingCalendarsResponse>> Handle(
        GetAllTradingCalendarsQuery request,
        CancellationToken cancellationToken)
    {
        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<TradingCalendar> page =
            await _calendarRepository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        TradingCalendarListItemResponse[] items =
            page.Items
                .Select(calendar => new TradingCalendarListItemResponse(
                    calendar.Id.Value.ToString(),
                    calendar.MarketId.Value.ToString(),
                    calendar.Year,
                    (int)calendar.WeekendDays,
                    calendar.IsActive))
                .ToArray();

        GetAllTradingCalendarsResponse response = new(
            items,
            page.Page,
            page.PageSize,
            page.TotalCount,
            page.TotalPages);

        return Result<GetAllTradingCalendarsResponse>.Success(response);
    }
}
