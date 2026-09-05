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
using MarketBook.Application.Features.Currencies.Commands.ActivateCurrency;
using MarketBook.Application.Features.Currencies.Commands.CreateCurrency;
using MarketBook.Application.Features.Currencies.Commands.DeactivateCurrency;
using MarketBook.Application.Features.Currencies.Commands.UpdateCurrency;
using MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies;
using MarketBook.Application.Features.Currencies.Queries.GetCurrencyById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for currency operations.
/// FA: نقاط پایانی HTTP مربوط به عملیات ارز را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/currencies")]
public sealed class CurrenciesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CurrenciesController"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="CurrenciesController"/> ایجاد می‌کند.
    /// </summary>
    public CurrenciesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a new currency.
    /// FA: یک ارز جدید ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        CreateCurrencyCommand command = new(request.Code, request.Name, request.DecimalPlaces);
        Result<CurrencyId> result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(Create),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Updates an existing currency.
    /// FA: یک ارز موجود را به‌روزرسانی می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        UpdateCurrencyCommand command = new(id, request.Code, request.Name, request.DecimalPlaces);
        Result<CurrencyId> result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Activates an existing currency.
    /// FA: یک ارز موجود را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(string id, CancellationToken cancellationToken)
    {
        Result<CurrencyId> result = await _sender.Send(new ActivateCurrencyCommand(id), cancellationToken);
        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates an existing currency.
    /// FA: یک ارز موجود را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(string id, CancellationToken cancellationToken)
    {
        Result<CurrencyId> result = await _sender.Send(new DeactivateCurrencyCommand(id), cancellationToken);
        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a currency by identifier.
    /// FA: یک ارز را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCurrencyByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        Result<GetCurrencyByIdResponse> result =
            await _sender.Send(new GetCurrencyByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a paged collection of currencies.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از ارزها را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllCurrenciesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllCurrenciesResponse> result =
            await _sender.Send(new GetAllCurrenciesQuery(page, pageSize), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
