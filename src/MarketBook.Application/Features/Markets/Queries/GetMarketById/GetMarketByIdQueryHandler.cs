using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Queries.GetMarketById;

/// <summary>
/// EN: Handles retrieving a Market aggregate by its identifier.
/// FA: دریافت Aggregate بازار بر اساس شناسه آن را مدیریت می‌کند.
/// </summary>
public sealed class GetMarketByIdQueryHandler
    : IRequestHandler<GetMarketByIdQuery, Result<GetMarketByIdResponse>>
{
    private readonly IMarketRepository _marketRepository;

    /// <summary>
    /// EN: Initializes a new instance of the market retrieval handler.
    /// FA: یک نمونه جدید از Handler دریافت بازار را ایجاد می‌کند.
    /// </summary>
    public GetMarketByIdQueryHandler(
        IMarketRepository marketRepository)
    {
        ArgumentNullException.ThrowIfNull(marketRepository);

        _marketRepository = marketRepository;
    }

    /// <summary>
    /// EN: Retrieves a market by its identifier.
    /// FA: بازار را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    public async Task<Result<GetMarketByIdResponse>> Handle(
        GetMarketByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!MarketId.TryParse(
                request.Id,
                out MarketId? marketId) ||
            marketId is null)
        {
            return Result<GetMarketByIdResponse>.Fail(
                new Error(
                    "Market.InvalidId",
                    "The specified market identifier is invalid."));
        }

        Market? market =
            await _marketRepository.GetByIdAsync(
                marketId,
                cancellationToken);

        if (market is null)
        {
            return Result<GetMarketByIdResponse>.Fail(
                new Error(
                    "Market.NotFound",
                    "The specified market was not found."));
        }

        GetMarketByIdResponse response =
            new(
                market.Id.Value.ToString(),
                market.Code.Value,
                market.Name,
                market.ExchangeId?.Value.ToString(),
                market.CreatedOn,
                market.IsActive);

        return Result<GetMarketByIdResponse>.Success(response);
    }
}
