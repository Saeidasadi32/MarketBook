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
using MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick;
using MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById;
using MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks;
using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides intraday price-tick endpoints.
/// FA: Endpointهای Tick قیمت درون‌روزی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/intraday-price-ticks")]
public sealed class IntradayPriceTicksController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public IntradayPriceTicksController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates one immutable intraday price tick.
    /// FA: یک Tick قیمت درون‌روزی تغییرناپذیر ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateIntradayPriceTickRequest request,
        CancellationToken cancellationToken)
    {
        CreateIntradayPriceTickCommand command = new(
            request.ListingId,
            request.TradingDate,
            request.OccurredAt,
            request.SequenceNumber,
            request.Price,
            request.Volume);

        Result<IntradayPriceTickId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets one intraday price tick by identifier.
    /// FA: یک Tick قیمت درون‌روزی را با شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetIntradayPriceTickByIdResponse> result =
            await _sender.Send(
                new GetIntradayPriceTickByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets paged intraday ticks for one Listing and trading date.
    /// FA: Tickهای یک Listing و تاریخ معاملاتی را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string listingId,
        [FromQuery] DateOnly tradingDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetIntradayPriceTicksResponse> result =
            await _sender.Send(
                new GetIntradayPriceTicksQuery(
                    listingId,
                    tradingDate,
                    page,
                    pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
