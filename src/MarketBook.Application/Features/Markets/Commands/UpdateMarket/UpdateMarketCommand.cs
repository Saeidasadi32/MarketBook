using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.UpdateMarket;

/// <summary>
/// EN: Represents a request to update an existing market.
/// FA: درخواست به‌روزرسانی یک بازار موجود را نمایش می‌دهد.
/// </summary>
public sealed record UpdateMarketCommand(
    string Id,
    string Code,
    string Name,
    string? ExchangeId) : IRequest<Result<MarketId>>;
