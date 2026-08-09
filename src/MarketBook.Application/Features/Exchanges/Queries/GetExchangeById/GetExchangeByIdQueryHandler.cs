// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Queries.GetExchangeById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Exchanges.Mappings;
using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;

namespace MarketBook.Application.Features.Exchanges.Queries.GetExchangeById;

/// <summary>
/// EN: Handles the GetExchangeByIdQuery.
/// FA: پرس‌وجوی دریافت بورس بر اساس شناسه را پردازش می‌کند.
/// </summary>
public sealed class GetExchangeByIdQueryHandler
    : IQueryHandler<
        GetExchangeByIdQuery,
        Result<ExchangeResponse>>
{
    private readonly IExchangeRepository _repository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repository"></param>
    public GetExchangeByIdQueryHandler(
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
    public async Task<Result<ExchangeResponse>> Handle(
        GetExchangeByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ExchangeId.TryParse(
                request.Id,
                out ExchangeId? id) ||
            id is null)
        {
            return Result<ExchangeResponse>.Fail(
                new Error(
                    "Exchange.InvalidId",
                    "The specified exchange identifier is invalid."));
        }

        Exchange? exchange = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (exchange is null)
        {
            return Result<ExchangeResponse>.Fail(
                new Error(
                    "Exchange.NotFound",
                    "Exchange was not found."));
        }

        return Result<ExchangeResponse>.Success(
            exchange.ToResponse());
    }
}
