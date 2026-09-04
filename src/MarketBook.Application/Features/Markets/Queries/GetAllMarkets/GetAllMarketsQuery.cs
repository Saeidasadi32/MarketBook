using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Markets.Queries.GetAllMarkets;

/// <summary>
/// EN: Represents a request to retrieve a paginated list of markets.
/// FA: درخواست دریافت فهرست صفحه‌بندی‌شده بازارها را نمایش می‌دهد.
/// </summary>
public sealed record GetAllMarketsQuery(
    int Page = 1,
    int PageSize = 20)
    : IRequest<Result<GetAllMarketsResponse>>;
