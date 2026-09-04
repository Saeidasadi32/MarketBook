using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Queries.GetMarketById;

/// <summary>
/// EN: Represents a request to retrieve a market by its identifier.
/// FA: درخواست دریافت یک بازار بر اساس شناسه آن را نمایش می‌دهد.
/// </summary>
public sealed record GetMarketByIdQuery(
    string Id) : IRequest<Result<GetMarketByIdResponse>>;
