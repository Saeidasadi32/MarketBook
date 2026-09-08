// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using IntradayPriceTickAggregate = MarketBook.Domain.MarketData.Aggregates.IntradayPriceTick;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById;

/// <summary>
/// EN: Handles retrieval of one intraday price tick.
/// FA: دریافت یک Tick قیمت درون‌روزی را مدیریت می‌کند.
/// </summary>
public sealed class GetIntradayPriceTickByIdHandler
    : IRequestHandler<GetIntradayPriceTickByIdQuery, Result<GetIntradayPriceTickByIdResponse>>
{
    private readonly IIntradayPriceTickRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetIntradayPriceTickByIdHandler(IIntradayPriceTickRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetIntradayPriceTickByIdResponse>> Handle(
        GetIntradayPriceTickByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!IntradayPriceTickId.TryParse(
                request.Id,
                out IntradayPriceTickId? id) ||
            id is null)
        {
            return Result<GetIntradayPriceTickByIdResponse>.Fail(
                new Error(
                    "IntradayPriceTick.InvalidId",
                    "The tick identifier is invalid."));
        }

        IntradayPriceTickAggregate? tick =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (tick is null)
        {
            return Result<GetIntradayPriceTickByIdResponse>.Fail(
                new Error(
                    "IntradayPriceTick.NotFound",
                    "The intraday price tick was not found."));
        }

        return Result<GetIntradayPriceTickByIdResponse>.Success(
            new GetIntradayPriceTickByIdResponse(
                tick.Id.Value.ToString(),
                tick.ListingId.Value.ToString(),
                tick.TradingDate,
                tick.OccurredAt,
                tick.SequenceNumber,
                tick.Price,
                tick.Volume,
                tick.TradeValue,
                tick.CreatedOn));
    }
}
