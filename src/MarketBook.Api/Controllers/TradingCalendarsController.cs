// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// Namespace : MarketBook.Api.Controllers
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Api.Common;
using MarketBook.Application.Features.TradingCalendars.Commands.ActivateTradingCalendar;
using MarketBook.Application.Features.TradingCalendars.Commands.CreateTradingCalendar;
using MarketBook.Application.Features.TradingCalendars.Commands.DeactivateTradingCalendar;
using MarketBook.Application.Features.TradingCalendars.Commands.UpdateTradingCalendar;
using MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay;
using MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars;
using MarketBook.Application.Features.TradingCalendars.Queries.GetTradingCalendarById;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for yearly trading calendars and sessions.
/// FA: Endpointهای HTTP تقویم‌های معاملاتی سالانه و Sessionها را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/trading-calendars")]
public sealed class TradingCalendarsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public TradingCalendarsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a yearly trading calendar.
    /// FA: تقویم معاملاتی سالانه ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTradingCalendarRequest request,
        CancellationToken cancellationToken)
    {
        CreateTradingCalendarCommand command = new(
            request.MarketId,
            request.Year,
            request.WeekendDays,
            request.Sessions,
            request.DateExceptions);

        Result<TradingCalendarId> result =
            await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Replaces mutable weekend/session/date-exception configuration.
    /// FA: تنظیمات قابل تغییر آخرهفته، Sessionها و استثناهای تاریخی را جایگزین می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateTradingCalendarRequest request,
        CancellationToken cancellationToken)
    {
        UpdateTradingCalendarCommand command = new(
            id,
            request.WeekendDays,
            request.Sessions,
            request.DateExceptions);

        Result<TradingCalendarId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a trading calendar by identifier.
    /// FA: تقویم معاملاتی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetTradingCalendarByIdResponse> result =
            await _sender.Send(
                new GetTradingCalendarByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets paged trading calendars.
    /// FA: تقویم‌های معاملاتی را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllTradingCalendarsResponse> result =
            await _sender.Send(
                new GetAllTradingCalendarsQuery(page, pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Evaluates whether a date is tradable and returns its regular session.
    /// FA: معاملاتی بودن یک تاریخ و Session عادی آن را ارزیابی می‌کند.
    /// </summary>
    [HttpGet("{id}/days/{date}")]
    public async Task<IActionResult> EvaluateDay(
        string id,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        Result<EvaluateTradingDayResponse> result =
            await _sender.Send(
                new EvaluateTradingDayQuery(id, date),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Activates a trading calendar.
    /// FA: تقویم معاملاتی را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<TradingCalendarId> result =
            await _sender.Send(
                new ActivateTradingCalendarCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates a trading calendar.
    /// FA: تقویم معاملاتی را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<TradingCalendarId> result =
            await _sender.Send(
                new DeactivateTradingCalendarCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
