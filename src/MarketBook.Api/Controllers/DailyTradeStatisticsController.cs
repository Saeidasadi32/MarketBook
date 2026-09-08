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
using MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById;
using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides daily trade-statistics endpoints.
/// FA: Endpointهای آمار معاملات روزانه را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/daily-trade-statistics")]
public sealed class DailyTradeStatisticsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public DailyTradeStatisticsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates daily trade statistics.
    /// FA: آمار معاملات روزانه را ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDailyTradeStatisticsRequest request,
        CancellationToken cancellationToken)
    {
        CreateDailyTradeStatisticsCommand command = new(
            request.ListingId,
            request.TradingDate,
            request.Volume,
            request.TradeCount,
            request.AveragePrice,
            request.TradeValue,
            request.MarketCapitalization);

        Result<DailyTradeStatisticsId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Updates daily trade statistics.
    /// FA: آمار معاملات روزانه را به‌روزرسانی می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateDailyTradeStatisticsRequest request,
        CancellationToken cancellationToken)
    {
        UpdateDailyTradeStatisticsCommand command = new(
            id,
            request.Volume,
            request.TradeCount,
            request.AveragePrice,
            request.TradeValue,
            request.MarketCapitalization);

        Result<DailyTradeStatisticsId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets daily trade statistics by identifier.
    /// FA: آمار معاملات روزانه را با شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetDailyTradeStatisticsByIdResponse> result =
            await _sender.Send(
                new GetDailyTradeStatisticsByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets paged daily trade statistics.
    /// FA: آمار معاملات روزانه را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllDailyTradeStatisticsResponse> result =
            await _sender.Send(
                new GetAllDailyTradeStatisticsQuery(page, pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
