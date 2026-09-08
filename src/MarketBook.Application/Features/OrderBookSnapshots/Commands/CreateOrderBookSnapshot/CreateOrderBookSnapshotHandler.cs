// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Entities;
using MarketBook.Domain.MarketData.ValueObjects;
using OrderBookSnapshotAggregate = MarketBook.Domain.MarketData.Aggregates.OrderBookSnapshot;
using MediatR;

namespace MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot;

/// <summary>EN: Handles order-book snapshot creation. FA: ایجاد Snapshot دفتر سفارشات را مدیریت می‌کند.</summary>
public sealed class CreateOrderBookSnapshotHandler
    : IRequestHandler<CreateOrderBookSnapshotCommand, Result<OrderBookSnapshotId>>
{
    private readonly IOrderBookSnapshotRepository _repository;
    private readonly IListingRepository _listingRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public CreateOrderBookSnapshotHandler(
        IOrderBookSnapshotRepository repository,
        IListingRepository listingRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _repository = repository;
        _listingRepository = listingRepository;
        _dbContext = dbContext;
    }

    /// <summary>EN: Handles the create command. FA: فرمان ایجاد را پردازش می‌کند.</summary>
    public async Task<Result<OrderBookSnapshotId>> Handle(
        CreateOrderBookSnapshotCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<OrderBookSnapshotId>.Fail(
                new Error(
                    "OrderBookSnapshot.InvalidListingId",
                    "The listing identifier is invalid."));
        }

        Listing? listing = await _listingRepository.GetByIdAsync(
            listingId,
            cancellationToken);

        if (listing is null)
        {
            return Result<OrderBookSnapshotId>.Fail(
                new Error(
                    "OrderBookSnapshot.ListingNotFound",
                    "The listing was not found."));
        }

        if (!listing.IsActive)
        {
            return Result<OrderBookSnapshotId>.Fail(
                new Error(
                    "OrderBookSnapshot.ListingInactive",
                    "The listing is inactive."));
        }

        if (await _repository.ExistsAsync(
                listingId,
                request.TradingDate,
                request.SequenceNumber,
                cancellationToken))
        {
            return Result<OrderBookSnapshotId>.Fail(
                new Error(
                    "OrderBookSnapshot.DuplicateSequence",
                    "A snapshot already exists for this Listing, trading date, and sequence number."));
        }

        OrderBookSnapshotAggregate snapshot;

        try
        {
            List<OrderBookSnapshotLevel> levels = request.Levels
                .Select(item => new OrderBookSnapshotLevel(
                    item.Level,
                    item.BidPrice,
                    item.BidVolume,
                    item.BidOrderCount,
                    item.AskPrice,
                    item.AskVolume,
                    item.AskOrderCount))
                .ToList();

            snapshot = OrderBookSnapshotAggregate.Create(
                listingId,
                request.TradingDate,
                request.CapturedAt,
                request.SequenceNumber,
                levels);
        }
        catch (ArgumentException exception)
        {
            return Result<OrderBookSnapshotId>.Fail(
                new Error(
                    "OrderBookSnapshot.InvalidSnapshot",
                    exception.Message));
        }

        await _repository.AddAsync(snapshot, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<OrderBookSnapshotId>.Success(snapshot.Id);
    }
}
