// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using DailyTradeStatisticsAggregate = MarketBook.Domain.MarketData.Aggregates.DailyTradeStatistics;
using MediatR;

namespace MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics;

/// <summary>
/// EN: Handles paged daily trade-statistics lookup.
/// FA: دریافت صفحه‌بندی‌شده آمار معاملات روزانه را مدیریت می‌کند.
/// </summary>
public sealed class GetAllDailyTradeStatisticsHandler
    : IRequestHandler<GetAllDailyTradeStatisticsQuery, Result<GetAllDailyTradeStatisticsResponse>>
{
    private readonly IDailyTradeStatisticsRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetAllDailyTradeStatisticsHandler(
        IDailyTradeStatisticsRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetAllDailyTradeStatisticsResponse>> Handle(
        GetAllDailyTradeStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };
        PagedResult<DailyTradeStatisticsAggregate> page =
            await _repository.GetPagedAsync(pageRequest, cancellationToken);

        List<DailyTradeStatisticsListItemResponse> items = page.Items
            .Select(static statistics => new DailyTradeStatisticsListItemResponse(
                statistics.Id.Value.ToString(),
                statistics.ListingId.Value.ToString(),
                statistics.TradingDate,
                statistics.Volume,
                statistics.TradeCount,
                statistics.TradeValue,
                statistics.MarketCapitalization))
            .ToList();

        GetAllDailyTradeStatisticsResponse response = new(
            items,
            page.Page,
            page.PageSize,
            page.TotalCount,
            page.TotalPages);

        return Result<GetAllDailyTradeStatisticsResponse>.Success(response);
    }
}
