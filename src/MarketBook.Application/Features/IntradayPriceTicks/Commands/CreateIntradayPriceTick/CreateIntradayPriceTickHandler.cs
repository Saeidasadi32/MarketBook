// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.ValueObjects;
using IntradayPriceTickAggregate = MarketBook.Domain.MarketData.Aggregates.IntradayPriceTick;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick;

/// <summary>
/// EN: Handles intraday price-tick creation.
/// FA: ایجاد Tick قیمت درون‌روزی را مدیریت می‌کند.
/// </summary>
public sealed class CreateIntradayPriceTickHandler
    : IRequestHandler<CreateIntradayPriceTickCommand, Result<IntradayPriceTickId>>
{
    private readonly IIntradayPriceTickRepository _repository;
    private readonly IListingRepository _listingRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreateIntradayPriceTickHandler(
        IIntradayPriceTickRepository repository,
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

    /// <summary>
    /// EN: Handles the create command.
    /// FA: فرمان ایجاد را پردازش می‌کند.
    /// </summary>
    public async Task<Result<IntradayPriceTickId>> Handle(
        CreateIntradayPriceTickCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<IntradayPriceTickId>.Fail(
                new Error(
                    "IntradayPriceTick.InvalidListingId",
                    "The listing identifier is invalid."));
        }

        Listing? listing = await _listingRepository.GetByIdAsync(
            listingId,
            cancellationToken);

        if (listing is null)
        {
            return Result<IntradayPriceTickId>.Fail(
                new Error(
                    "IntradayPriceTick.ListingNotFound",
                    "The listing was not found."));
        }

        if (!listing.IsActive)
        {
            return Result<IntradayPriceTickId>.Fail(
                new Error(
                    "IntradayPriceTick.ListingInactive",
                    "The listing is inactive."));
        }

        if (await _repository.ExistsAsync(
                listingId,
                request.TradingDate,
                request.SequenceNumber,
                cancellationToken))
        {
            return Result<IntradayPriceTickId>.Fail(
                new Error(
                    "IntradayPriceTick.DuplicateSequence",
                    "A tick already exists for this Listing, trading date, and sequence number."));
        }

        IntradayPriceTickAggregate tick;

        try
        {
            tick = IntradayPriceTickAggregate.Create(
                listingId,
                request.TradingDate,
                request.OccurredAt,
                request.SequenceNumber,
                request.Price,
                request.Volume);
        }
        catch (ArgumentException exception)
        {
            return Result<IntradayPriceTickId>.Fail(
                new Error(
                    "IntradayPriceTick.InvalidTick",
                    exception.Message));
        }

        await _repository.AddAsync(tick, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<IntradayPriceTickId>.Success(tick.Id);
    }
}
