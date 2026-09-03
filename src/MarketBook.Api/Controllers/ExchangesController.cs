// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : API
// Namespace : MarketBook.Api.Controllers
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Api.Common;
using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Exchanges.Commands.CreateExchange;
using MarketBook.Application.Features.Exchanges.Commands.DeactivateExchange;
using MarketBook.Application.Features.Exchanges.Commands.UpdateExchange;
using MarketBook.Application.Features.Exchanges.Queries.GetExchangeById;
using MarketBook.Application.Features.Exchanges.Queries.GetExchanges;
using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides endpoints for exchange management.
/// FA: نقاط دسترسی مدیریت بورس‌ها را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/exchanges")]
public sealed class ExchangesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangesController"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ExchangesController"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="sender">
    /// EN: MediatR request sender.
    /// FA: ارسال‌کننده درخواست‌های MediatR.
    /// </param>
    public ExchangesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);

        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a new exchange.
    /// FA: یک بورس جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="command">
    /// EN: Exchange creation command.
    /// FA: فرمان ایجاد بورس.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the created exchange.
    /// FA: شناسه بورس ایجادشده.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExchangeCommand command,
        CancellationToken cancellationToken)
    {
        Result<ExchangeId> result = await _sender
            .Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return ApiErrorMapper.ToActionResult(
                this,
                result.Error);
        }

        string id = result.Value!.ToString();

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new
            {
                id
            });
    }

    /// <summary>
    /// EN: Updates an existing exchange.
    /// FA: یک بورس موجود را به‌روزرسانی می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Exchange identifier.
    /// FA: شناسه بورس.
    /// </param>
    /// <param name="request">
    /// EN: Exchange update command.
    /// FA: فرمان به‌روزرسانی بورس.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the updated exchange.
    /// FA: شناسه بورس به‌روزرسانی‌شده.
    /// </returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateExchangeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(request);

        UpdateExchangeCommand command = new(
            id,
            request.Code,
            request.Name,
            request.CountryId);

        Result<ExchangeId> result = await _sender
            .Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return ApiErrorMapper.ToActionResult(
                this,
                result.Error);
        }

        return Ok(new
        {
            id = result.Value!.Value.ToString()
        });
    }

    /// <summary>
    /// EN: Deactivates an existing exchange.
    /// FA: یک بورس موجود را غیرفعال می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Exchange identifier.
    /// FA: شناسه بورس.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the deactivated exchange.
    /// FA: شناسه بورس غیرفعال‌شده.
    /// </returns>
    /// <response code="200">
    /// EN: Exchange was successfully deactivated.
    /// FA: بورس با موفقیت غیرفعال شد.
    /// </response>
    /// <response code="400">
    /// EN: The exchange identifier is invalid.
    /// FA: شناسه بورس نامعتبر است.
    /// </response>
    /// <response code="404">
    /// EN: The specified exchange was not found.
    /// FA: بورس موردنظر یافت نشد.
    /// </response>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        DeactivateExchangeCommand command = new(id);

        Result<ExchangeId> result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                id = result.Value!.Value.ToString()
            });
        }

        return ApiErrorMapper.ToActionResult(
            this,
            result.Error);
    }

    /// <summary>
    /// EN: Gets an exchange by identifier.
    /// FA: یک بورس را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Exchange identifier.
    /// FA: شناسه بورس.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The requested exchange.
    /// FA: بورس مورد درخواست.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        GetExchangeByIdQuery query = new(id);

        Result<ExchangeResponse> result = await _sender
            .Send(query, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return ApiErrorMapper.ToActionResult(
                this,
                result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// EN: Gets exchanges with pagination.
    /// FA: بورس‌ها را به صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    /// <param name="page">
    /// EN: Page number.
    /// FA: شماره صفحه.
    /// </param>
    /// <param name="pageSize">
    /// EN: Number of items per page.
    /// FA: تعداد آیتم‌های هر صفحه.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paginated list of exchanges.
    /// FA: فهرست صفحه‌بندی‌شده بورس‌ها.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = PageRequest.DefaultPage,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        GetExchangesQuery query = new(
            page,
            pageSize);

        Result<PagedResult<ExchangeResponse>> result =
            await _sender
                .Send(query, cancellationToken)
                .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return ApiErrorMapper.ToActionResult(
                this,
                result.Error);
        }

        return Ok(result.Value);
    }
}
