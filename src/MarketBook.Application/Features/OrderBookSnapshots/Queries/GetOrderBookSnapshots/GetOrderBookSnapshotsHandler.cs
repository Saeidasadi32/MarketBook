// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Entities;
using OrderBookSnapshotAggregate = MarketBook.Domain.MarketData.Aggregates.OrderBookSnapshot;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots;

/// <summary>EN: Handles paged order-book snapshot queries. FA: پرس‌وجوهای صفحه‌بندی Snapshotهای دفتر سفارشات را مدیریت می‌کند.</summary>
public sealed class GetOrderBookSnapshotsHandler
    : IRequestHandler<GetOrderBookSnapshotsQuery, Result<GetOrderBookSnapshotsResponse>>
{
    private readonly IOrderBookSnapshotRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetOrderBookSnapshotsHandler(IOrderBookSnapshotRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>EN: Handles the query. FA: پرس‌وجو را پردازش می‌کند.</summary>
    public async Task<Result<GetOrderBookSnapshotsResponse>> Handle(
        GetOrderBookSnapshotsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<GetOrderBookSnapshotsResponse>.Fail(
                new Error(
                    "OrderBookSnapshot.InvalidListingId",
                    "The listing identifier is invalid."));
        }

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<OrderBookSnapshotAggregate> page =
            await _repository.GetPagedAsync(
                listingId,
                request.TradingDate,
                pageRequest,
                cancellationToken);

        List<OrderBookSnapshotItem> items = page.Items
            .Select(snapshot => new OrderBookSnapshotItem(
                snapshot.Id.Value.ToString(),
                snapshot.ListingId.Value.ToString(),
                snapshot.TradingDate,
                snapshot.CapturedAt,
                snapshot.SequenceNumber,
                snapshot.Levels.Select(MapLevel).ToList()))
            .ToList();

        return Result<GetOrderBookSnapshotsResponse>.Success(
            new GetOrderBookSnapshotsResponse(
                items,
                page.Page,
                page.PageSize,
                page.TotalCount,
                page.TotalPages));
    }

    private static OrderBookSnapshotItemLevel MapLevel(
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
