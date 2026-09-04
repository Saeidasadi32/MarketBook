using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Markets.Queries.GetAllMarkets;

/// <summary>
/// EN: Handles retrieving a paginated list of Market aggregates.
/// FA: دریافت فهرست صفحه‌بندی‌شده Aggregateهای بازار را مدیریت می‌کند.
/// </summary>
public sealed class GetAllMarketsQueryHandler
    : IRequestHandler<GetAllMarketsQuery, Result<GetAllMarketsResponse>>
{
    private readonly IMarketRepository _marketRepository;

    /// <summary>
    /// EN: Initializes a new instance of the market list handler.
    /// FA: یک نمونه جدید از Handler فهرست بازارها را ایجاد می‌کند.
    /// </summary>
    public GetAllMarketsQueryHandler(
        IMarketRepository marketRepository)
    {
        ArgumentNullException.ThrowIfNull(marketRepository);

        _marketRepository = marketRepository;
    }

    /// <summary>
    /// EN: Retrieves markets using pagination.
    /// FA: بازارها را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.
    /// </summary>
    public async Task<Result<GetAllMarketsResponse>> Handle(
        GetAllMarketsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest;

        pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Market> result =
            await _marketRepository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        List<MarketListItemResponse> items =
            result.Items
                .Select(market =>
                    new MarketListItemResponse(
                        market.Id.Value.ToString(),
                        market.Code.Value,
                        market.Name,
                        market.ExchangeId?.Value.ToString(),
                        market.CreatedOn,
                        market.IsActive))
                .ToList();

        GetAllMarketsResponse response =
            new(
                items,
                result.Page,
                result.PageSize,
                result.TotalCount,
                result.TotalPages);

        return Result<GetAllMarketsResponse>.Success(response);
    }
}
