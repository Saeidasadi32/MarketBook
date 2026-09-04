namespace MarketBook.Application.Features.Markets.Queries.GetAllMarkets;

/// <summary>
/// EN: Represents a paginated response containing markets.
/// FA: پاسخ صفحه‌بندی‌شده شامل بازارها را نمایش می‌دهد.
/// </summary>
public sealed record GetAllMarketsResponse(
    IReadOnlyCollection<MarketListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents a market item returned in a market list.
/// FA: یک مورد بازار را در فهرست بازارها نمایش می‌دهد.
/// </summary>
public sealed record MarketListItemResponse(
    string Id,
    string Code,
    string Name,
    string? ExchangeId,
    DateTimeOffset CreatedOn,
    bool IsActive);
