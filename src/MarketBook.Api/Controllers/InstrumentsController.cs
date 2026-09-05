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
using MarketBook.Application.Features.Instruments.Commands.ActivateInstrument;
using MarketBook.Application.Features.Instruments.Commands.CreateInstrument;
using MarketBook.Application.Features.Instruments.Commands.DeactivateInstrument;
using MarketBook.Application.Features.Instruments.Commands.UpdateInstrument;
using MarketBook.Application.Features.Instruments.Queries.GetAllInstruments;
using MarketBook.Application.Features.Instruments.Queries.GetInstrumentById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for instrument operations.
/// FA: نقاط پایانی HTTP مربوط به عملیات ابزار مالی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/instruments")]
public sealed class InstrumentsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes a new controller.
    /// FA: Controller جدید را مقداردهی می‌کند.
    /// </summary>
    public InstrumentsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a new instrument.
    /// FA: ابزار مالی جدید ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        CreateInstrumentCommand command = new(
            request.Name,
            request.AssetClass,
            request.Type,
            request.Category,
            request.Isin);

        Result<InstrumentId> result =
            await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(Create),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Updates an existing instrument.
    /// FA: ابزار مالی موجود را به‌روزرسانی می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        UpdateInstrumentCommand command = new(
            id,
            request.Name,
            request.AssetClass,
            request.Type,
            request.Category,
            request.Isin);

        Result<InstrumentId> result =
            await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(
                new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Activates an instrument.
    /// FA: ابزار مالی را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<InstrumentId> result =
            await _sender.Send(
                new ActivateInstrumentCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates an instrument.
    /// FA: ابزار مالی را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<InstrumentId> result =
            await _sender.Send(
                new DeactivateInstrumentCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets an instrument by identifier.
    /// FA: ابزار مالی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetInstrumentByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetInstrumentByIdResponse> result =
            await _sender.Send(
                new GetInstrumentByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a paged collection of instruments.
    /// FA: مجموعه صفحه‌بندی‌شده ابزارهای مالی را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllInstrumentsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllInstrumentsResponse> result =
            await _sender.Send(
                new GetAllInstrumentsQuery(page, pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
