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
using MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot;
using MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById;
using MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots;
using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>EN: Provides order-book snapshot endpoints. FA: Endpointهای Snapshot دفتر سفارشات را فراهم می‌کند.</summary>
[ApiController]
[Route("api/v1/order-book-snapshots")]
public sealed class OrderBookSnapshotsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>EN: Initializes the controller. FA: Controller را مقداردهی می‌کند.</summary>
    public OrderBookSnapshotsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>EN: Creates one immutable order-book snapshot. FA: یک Snapshot تغییرناپذیر دفتر سفارشات ایجاد می‌کند.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderBookSnapshotRequest request,
        CancellationToken cancellationToken)
    {
        CreateOrderBookSnapshotCommand command = new(
            request.ListingId,
            request.TradingDate,
            request.CapturedAt,
            request.SequenceNumber,
            request.Levels);

        Result<OrderBookSnapshotId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets one order-book snapshot by identifier. FA: یک Snapshot دفتر سفارشات را با شناسه دریافت می‌کند.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetOrderBookSnapshotByIdResponse> result =
            await _sender.Send(
                new GetOrderBookSnapshotByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets paged snapshots for one Listing and trading date. FA: Snapshotهای یک Listing و تاریخ معاملاتی را صفحه‌بندی‌شده دریافت می‌کند.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string listingId,
        [FromQuery] DateOnly tradingDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetOrderBookSnapshotsResponse> result =
            await _sender.Send(
                new GetOrderBookSnapshotsQuery(
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
