// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using DailyTradeStatisticsAggregate = MarketBook.Domain.MarketData.Aggregates.DailyTradeStatistics;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics;

/// <summary>
/// EN: Handles daily trade-statistics updates.
/// FA: به‌روزرسانی آمار معاملات روزانه را مدیریت می‌کند.
/// </summary>
public sealed class UpdateDailyTradeStatisticsHandler
    : IRequestHandler<UpdateDailyTradeStatisticsCommand, Result<DailyTradeStatisticsId>>
{
    private readonly IDailyTradeStatisticsRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public UpdateDailyTradeStatisticsHandler(
        IDailyTradeStatisticsRepository repository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _repository = repository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the update command.
    /// FA: فرمان به‌روزرسانی را پردازش می‌کند.
    /// </summary>
    public async Task<Result<DailyTradeStatisticsId>> Handle(
        UpdateDailyTradeStatisticsCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!DailyTradeStatisticsId.TryParse(
                request.Id,
                out DailyTradeStatisticsId? id) ||
            id is null)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.InvalidId",
                    "The identifier is invalid."));
        }

        DailyTradeStatisticsAggregate? statistics =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (statistics is null)
        {
            return Result<DailyTradeStatisticsId>.Fail(
                new Error(
                    "DailyTradeStatistics.NotFound",
                    "The daily trade-statistics record was not found."));
        }

        try
        {
            statistics.Update(
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

        _repository.Update(statistics);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<DailyTradeStatisticsId>.Success(statistics.Id);
    }
}
