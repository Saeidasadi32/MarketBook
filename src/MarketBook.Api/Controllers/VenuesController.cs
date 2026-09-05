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
using MarketBook.Application.Features.Venues.Commands.CreateVenue;
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
}
