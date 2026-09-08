// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.Entities;
using MarketBook.Domain.MarketData.ValueObjects;
using OrderBookSnapshotAggregate = MarketBook.Domain.MarketData.Aggregates.OrderBookSnapshot;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById;

/// <summary>EN: Handles retrieval of one order-book snapshot. FA: دریافت یک Snapshot دفتر سفارشات را مدیریت می‌کند.</summary>
public sealed class GetOrderBookSnapshotByIdHandler
    : IRequestHandler<GetOrderBookSnapshotByIdQuery, Result<GetOrderBookSnapshotByIdResponse>>
{
    private readonly IOrderBookSnapshotRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetOrderBookSnapshotByIdHandler(IOrderBookSnapshotRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>EN: Handles the query. FA: پرس‌وجو را پردازش می‌کند.</summary>
    public async Task<Result<GetOrderBookSnapshotByIdResponse>> Handle(
        GetOrderBookSnapshotByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!OrderBookSnapshotId.TryParse(
                request.Id,
                out OrderBookSnapshotId? id) ||
            id is null)
        {
            return Result<GetOrderBookSnapshotByIdResponse>.Fail(
                new Error(
                    "OrderBookSnapshot.InvalidId",
                    "The snapshot identifier is invalid."));
        }

        OrderBookSnapshotAggregate? snapshot =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (snapshot is null)
        {
            return Result<GetOrderBookSnapshotByIdResponse>.Fail(
                new Error(
                    "OrderBookSnapshot.NotFound",
                    "The order-book snapshot was not found."));
        }

        List<OrderBookSnapshotLevelResponse> levels = snapshot.Levels
            .Select(MapLevel)
            .ToList();

        return Result<GetOrderBookSnapshotByIdResponse>.Success(
            new GetOrderBookSnapshotByIdResponse(
                snapshot.Id.Value.ToString(),
                snapshot.ListingId.Value.ToString(),
                snapshot.TradingDate,
                snapshot.CapturedAt,
                snapshot.SequenceNumber,
                levels,
                snapshot.CreatedOn));
    }

    private static OrderBookSnapshotLevelResponse MapLevel(
        OrderBookSnapshotLevel level)
        => new(
            level.Level,
            level.BidPrice,
            level.BidVolume,
            level.BidOrderCount,
            level.AskPrice,
            level.AskVolume,
            level.AskOrderCount);
}
