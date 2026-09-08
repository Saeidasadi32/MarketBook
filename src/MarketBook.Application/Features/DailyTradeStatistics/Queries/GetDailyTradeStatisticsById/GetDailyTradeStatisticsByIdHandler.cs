// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using DailyTradeStatisticsAggregate = MarketBook.Domain.MarketData.Aggregates.DailyTradeStatistics;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById;

/// <summary>
/// EN: Handles daily trade-statistics lookup.
/// FA: دریافت آمار معاملات روزانه را مدیریت می‌کند.
/// </summary>
public sealed class GetDailyTradeStatisticsByIdHandler
    : IRequestHandler<GetDailyTradeStatisticsByIdQuery, Result<GetDailyTradeStatisticsByIdResponse>>
{
    private readonly IDailyTradeStatisticsRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetDailyTradeStatisticsByIdHandler(
        IDailyTradeStatisticsRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetDailyTradeStatisticsByIdResponse>> Handle(
        GetDailyTradeStatisticsByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!DailyTradeStatisticsId.TryParse(
                request.Id,
                out DailyTradeStatisticsId? id) ||
            id is null)
        {
            return Result<GetDailyTradeStatisticsByIdResponse>.Fail(
                new Error(
                    "DailyTradeStatistics.InvalidId",
                    "The identifier is invalid."));
        }

        DailyTradeStatisticsAggregate? statistics =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (statistics is null)
        {
            return Result<GetDailyTradeStatisticsByIdResponse>.Fail(
                new Error(
                    "DailyTradeStatistics.NotFound",
                    "The daily trade-statistics record was not found."));
        }

        GetDailyTradeStatisticsByIdResponse response = new(
            statistics.Id.Value.ToString(),
            statistics.ListingId.Value.ToString(),
            statistics.TradingDate,
            statistics.Volume,
            statistics.TradeCount,
            statistics.AveragePrice,
            statistics.TradeValue,
            statistics.MarketCapitalization,
            statistics.AverageTradeSize,
            statistics.CreatedOn,
            statistics.UpdatedOn);

        return Result<GetDailyTradeStatisticsByIdResponse>.Success(response);
    }
}
