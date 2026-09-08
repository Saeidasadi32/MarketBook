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
using MarketBook.Application.Features.FxRates.Commands.CreateFxRate;
using MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides FX-rate HTTP endpoints.
/// FA: Endpointهای HTTP نرخ ارز را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/fx-rates")]
public sealed class FxRatesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public FxRatesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a dated FX quote.
    /// FA: یک نرخ تاریخ‌دار ارز ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFxRateRequest request,
        CancellationToken cancellationToken)
    {
        Result<FxRateId> result =
            await _sender.Send(
                new CreateFxRateCommand(
                    request.BaseCurrencyId,
                    request.QuoteCurrencyId,
                    request.RateDate,
                    request.Rate),
                cancellationToken);

        return result.IsSuccess
            ? Created(
                $"/api/v1/fx-rates/{result.Value!.Value}",
                new { id = result.Value.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets the latest exact-direction FX quote.
    /// FA: آخرین نرخ ارز با جهت دقیق را دریافت می‌کند.
    /// </summary>
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(
        [FromQuery] string baseCurrencyId,
        [FromQuery] string quoteCurrencyId,
        CancellationToken cancellationToken = default)
    {
        Result<GetLatestFxRateResponse> result =
            await _sender.Send(
                new GetLatestFxRateQuery(
                    baseCurrencyId,
                    quoteCurrencyId),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
