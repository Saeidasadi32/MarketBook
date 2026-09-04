namespace MarketBook.Application.Features.Markets.Queries.GetMarketById;

/// <summary>
/// EN: Represents the data returned when a market is retrieved by its identifier.
/// FA: اطلاعات بازگردانده‌شده هنگام دریافت بازار بر اساس شناسه آن را نمایش می‌دهد.
/// </summary>
public sealed record GetMarketByIdResponse(
    string Id,
    string Code,
    string Name,
    string? ExchangeId,
    DateTimeOffset CreatedOn,
    bool IsActive);
