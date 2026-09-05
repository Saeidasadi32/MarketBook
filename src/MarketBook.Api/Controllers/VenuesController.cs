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
using MarketBook.Application.Features.Venues.Commands.ActivateVenue;
using MarketBook.Application.Features.Venues.Commands.CreateVenue;
using MarketBook.Application.Features.Venues.Commands.DeactivateVenue;
using MarketBook.Application.Features.Venues.Commands.UpdateVenue;
using MarketBook.Application.Features.Venues.Queries.GetAllVenues;
using MarketBook.Application.Features.Venues.Queries.GetVenueById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for trading venue operations.
/// FA: نقاط پایانی HTTP مربوط به عملیات بسترهای معاملاتی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/venues")]
public sealed class VenuesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="VenuesController"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="VenuesController"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="sender">
    /// EN: MediatR sender used to dispatch application commands and queries.
    /// FA: Sender مربوط به MediatR برای ارسال Command و Queryهای برنامه.
    /// </param>
    public VenuesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a new trading venue.
    /// FA: یک بستر معاملاتی جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Venue creation request.
    /// FA: درخواست ایجاد بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the newly created venue.
    /// FA: شناسه بستر معاملاتی ایجادشده.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        CreateVenueCommand command = new(
            request.MarketId,
            request.Code,
            request.Name,
            request.Type);

        Result<VenueId> result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(Create),
                new
                {
                    id = result.Value!.Value.ToString()
                },
                new
                {
                    id = result.Value!.Value.ToString()
                });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// Updates an existing venue.
    /// <para>
    /// یک محل معاملاتی موجود را به‌روزرسانی می‌کند.
    /// </para>
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        UpdateVenueCommand command = new(
            id,
            request.Name,
            request.Type);

        Result<VenueId> result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(
                new
                {
                    id = result.Value!.Value.ToString()
                });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Deactivates an existing venue.
    /// FA: یک محل معاملاتی موجود را غیرفعال می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Venue identifier.
    /// FA: شناسه محل معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: HTTP response containing the deactivated venue identifier or an error.
    /// FA: پاسخ HTTP شامل شناسه محل معاملاتی غیرفعال‌شده یا خطا.
    /// </returns>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        DeactivateVenueCommand command = new(id);

        Result<VenueId> result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(
                new
                {
                    id = result.Value!.Value.ToString()
                });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Activates an existing venue.
    /// FA: یک محل معاملاتی موجود را فعال می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Venue identifier.
    /// FA: شناسه محل معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: HTTP response containing the activated venue identifier or an error.
    /// FA: پاسخ HTTP شامل شناسه محل معاملاتی فعال‌شده یا خطا.
    /// </returns>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        ActivateVenueCommand command = new(id);

        Result<VenueId> result =
            await _sender.Send(
                command,
                cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(
                new
                {
                    id = result.Value!.Value.ToString()
                });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Gets a trading venue by its identifier.
    /// FA: یک بستر معاملاتی را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(
        typeof(GetVenueByIdResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        GetVenueByIdQuery query = new(id);

        Result<GetVenueByIdResponse> result =
            await _sender.Send(
                query,
                cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Gets a paged collection of trading venues.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از بسترهای معاملاتی را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
    typeof(GetAllVenuesResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        GetAllVenuesQuery query =
            new(
                page,
                pageSize);

        Result<GetAllVenuesResponse> result =
            await _sender.Send(
                query,
                cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }
}
