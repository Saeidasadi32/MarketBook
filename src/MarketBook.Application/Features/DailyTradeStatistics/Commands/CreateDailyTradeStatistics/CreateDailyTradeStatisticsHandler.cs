// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using DailyTradeStatisticsAggregate = MarketBook.Domain.MarketData.Aggregates.DailyTradeStatistics;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics;

/// <summary>
/// EN: Handles daily trade-statistics creation.
/// FA: ایجاد آمار معاملات روزانه را مدیریت می‌کند.
/// </summary>
public sealed class CreateDailyTradeStatisticsHandler
    : IRequestHandler<CreateDailyTradeStatisticsCommand, Result<DailyTradeStatisticsId>>
{
    private readonly IDailyTradeStatisticsRepository _repository;
    private readonly IListingRepository _listingRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreateDailyTradeStatisticsHandler(
        IDailyTradeStatisticsRepository repository,
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
    public async Task<Result<DailyTradeStatisticsId>> Handle(
        CreateDailyTradeStatisticsCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.InvalidListingId",
                    "The listing identifier is invalid."));
        }

        Listing? listing = await _listingRepository.GetByIdAsync(
            listingId,
            cancellationToken);

        if (listing is null)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.ListingNotFound",
                    "The listing was not found."));
        }

        if (!listing.IsActive)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.ListingInactive",
                    "The listing is inactive."));
        }

        if (await _repository.ExistsAsync(
                listingId,
                request.TradingDate,
                cancellationToken))
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.DuplicateListingDate",
                    "Daily trade statistics already exist for this Listing and trading date."));
        }

        DailyTradeStatisticsAggregate statistics;

        try
        {
            statistics = DailyTradeStatisticsAggregate.Create(
                listingId,
                request.TradingDate,
                request.Volume,
                request.TradeCount,
                request.AveragePrice,
                request.TradeValue,
                request.MarketCapitalization);
        }
        catch (ArgumentException exception)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.InvalidStatistics",
                    exception.Message));
        }

        await _repository.AddAsync(statistics, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<DailyTradeStatisticsId>.Success(statistics.Id);
    }
}
