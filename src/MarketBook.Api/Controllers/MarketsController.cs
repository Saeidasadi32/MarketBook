
using MarketBook.Api.Common;
using MarketBook.Application.Features.Markets.Commands.CreateMarket;
using MarketBook.Application.Features.Markets.Commands.DeactivateMarket;
using MarketBook.Application.Features.Markets.Commands.UpdateMarket;
using MarketBook.Application.Features.Markets.Queries.GetAllMarkets;
using MarketBook.Application.Features.Markets.Queries.GetMarketById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for managing markets.
/// FA: نقاط پایانی HTTP برای مدیریت بازارها را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/markets")]
public sealed class MarketsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes a new instance of the Markets controller.
    /// FA: یک نمونه جدید از Controller بازارها را ایجاد می‌کند.
    /// </summary>
    /// <param name="sender">
    /// EN: MediatR request sender.
    /// FA: ارسال‌کننده درخواست‌های MediatR.
    /// </param>
    public MarketsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a new market.
    /// FA: یک بازار جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Market creation request.
    /// FA: درخواست ایجاد بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the newly created market.
    /// FA: شناسه بازار ایجادشده.
    /// </returns>
    /// <response code="201">
    /// EN: Market was successfully created.
    /// FA: بازار با موفقیت ایجاد شد.
    /// </response>
    /// <response code="400">
    /// EN: The supplied market data is invalid.
    /// FA: اطلاعات ارائه‌شده برای بازار نامعتبر است.
    /// </response>
    /// <response code="409">
    /// EN: A market with the specified code already exists.
    /// FA: بازاری با کد مشخص‌شده از قبل وجود دارد.
    /// </response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMarketRequest request,
        CancellationToken cancellationToken)
    {
        CreateMarketCommand command = new(
            request.Code,
            request.Name,
            request.ExchangeId);

        Result<MarketId> result =
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
                    id = result.Value.Value.ToString()
                });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Updates an existing market.
    /// FA: یک بازار موجود را به‌روزرسانی می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateMarketRequest request,
        CancellationToken cancellationToken)
    {
        UpdateMarketCommand command = new(
            id,
            request.Code,
            request.Name,
            request.ExchangeId);

        Result<MarketId> result =
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

    // -----------------------------------------------------------------------------
    // Project   : MarketBook (Intelligent Market Book System)
    // Platform  : MarketBook Platform
    // Layer     : API
    // Namespace : MarketBook.Api.Controllers
    //
    // Copyright (c) Saeid Asadi. All rights reserved.
    // Licensed under the MIT License.
    // -----------------------------------------------------------------------------

    /// <summary>
    /// EN: Deactivates an existing market.
    /// FA: یک بازار موجود را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        DeactivateMarketCommand command = new(id);

        Result<MarketId> result =
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
    /// EN: Retrieves a market by its identifier.
    /// FA: یک بازار را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetMarketByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        GetMarketByIdQuery query = new(id);

        Result<GetMarketByIdResponse> result =
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
    /// EN: Retrieves a paginated list of markets.
    /// FA: فهرست صفحه‌بندی‌شده بازارها را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(GetAllMarketsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        GetAllMarketsQuery query = new(
            page,
            pageSize);

        Result<GetAllMarketsResponse> result =
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
