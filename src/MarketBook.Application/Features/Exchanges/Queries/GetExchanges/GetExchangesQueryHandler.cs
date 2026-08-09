// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Queries.GetExchanges
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Exchanges.Mappings;
using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.Aggregates;

namespace MarketBook.Application.Features.Exchanges.Queries.GetExchanges;

/// <summary>
/// EN: Handles the GetExchangesQuery.
/// FA: پرس‌وجوی دریافت بورس‌ها را پردازش می‌کند.
/// </summary>
public sealed class GetExchangesQueryHandler
    : IQueryHandler<
        GetExchangesQuery,
        Result<PagedResult<ExchangeResponse>>>
{
    private readonly IExchangeRepository _repository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repository"></param>
    public GetExchangesQueryHandler(
        IExchangeRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<PagedResult<ExchangeResponse>>> Handle(
        GetExchangesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Exchange> result =
            await _repository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        IReadOnlyList<ExchangeResponse> items =
            result.Items
                .Select(exchange => exchange.ToResponse())
                .ToList();

        PagedResult<ExchangeResponse> response =
            new(
                items,
                result.Page,
                result.PageSize,
                result.TotalCount);

        return Result<PagedResult<ExchangeResponse>>.Success(
            response);
    }
}
