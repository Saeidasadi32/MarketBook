// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using IntradayPriceTickAggregate = MarketBook.Domain.MarketData.Aggregates.IntradayPriceTick;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks;

/// <summary>
/// EN: Handles paged intraday-tick queries.
/// FA: پرس‌وجوهای صفحه‌بندی Tickهای درون‌روزی را مدیریت می‌کند.
/// </summary>
public sealed class GetIntradayPriceTicksHandler
    : IRequestHandler<GetIntradayPriceTicksQuery, Result<GetIntradayPriceTicksResponse>>
{
    private readonly IIntradayPriceTickRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetIntradayPriceTicksHandler(IIntradayPriceTickRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetIntradayPriceTicksResponse>> Handle(
        GetIntradayPriceTicksQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<GetIntradayPriceTicksResponse>.Fail(
                new Error(
                    "IntradayPriceTick.InvalidListingId",
                    "The listing identifier is invalid."));
        }

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<IntradayPriceTickAggregate> page =
            await _repository.GetPagedAsync(
                listingId,
                request.TradingDate,
                pageRequest,
                cancellationToken);

        List<IntradayPriceTickItem> items = page.Items
            .Select(tick => new IntradayPriceTickItem(
                tick.Id.Value.ToString(),
                tick.ListingId.Value.ToString(),
                tick.TradingDate,
                tick.OccurredAt,
                tick.SequenceNumber,
                tick.Price,
                tick.Volume,
                tick.TradeValue))
            .ToList();

        return Result<GetIntradayPriceTicksResponse>.Success(
            new GetIntradayPriceTicksResponse(
                items,
                page.Page,
                page.PageSize,
                page.TotalCount,
                page.TotalPages));
    }
}
