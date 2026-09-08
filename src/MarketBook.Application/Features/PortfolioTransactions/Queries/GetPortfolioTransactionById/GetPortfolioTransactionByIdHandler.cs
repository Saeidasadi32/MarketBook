// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById;

/// <summary>
/// EN: Handles transaction lookup by identifier.
/// FA: دریافت تراکنش با شناسه را مدیریت می‌کند.
/// </summary>
public sealed class GetPortfolioTransactionByIdHandler
    : IRequestHandler<GetPortfolioTransactionByIdQuery, Result<GetPortfolioTransactionByIdResponse>>
{
    private readonly IPortfolioTransactionRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioTransactionByIdHandler(IPortfolioTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the lookup query.
    /// FA: Query دریافت را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioTransactionByIdResponse>> Handle(
        GetPortfolioTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioTransactionId.TryParse(request.Id, out PortfolioTransactionId? id) ||
            id is null)
        {
            return Result<GetPortfolioTransactionByIdResponse>.Fail(
                new Error("PortfolioTransaction.InvalidId", "The transaction identifier is invalid."));
        }

        PortfolioTransaction? item =
            await _repository.GetByIdAsync(id, cancellationToken);

        if (item is null)
            return Result<GetPortfolioTransactionByIdResponse>.Fail(
                new Error("PortfolioTransaction.NotFound", "The portfolio transaction was not found."));

        return Result<GetPortfolioTransactionByIdResponse>.Success(
            new GetPortfolioTransactionByIdResponse(
                item.Id.Value.ToString(),
                item.PortfolioId.Value.ToString(),
                item.ListingId.Value.ToString(),
                item.CurrencyId.Value.ToString(),
                (int)item.Type,
                item.Quantity,
                item.Price,
                item.Commission,
                item.Tax,
                item.ExchangeFee,
                item.BrokerFee,
                item.ClearingFee,
                item.OtherFees,
                item.GrossValue,
                item.TotalCosts,
                item.ExecutedOn,
                item.CreatedOn));
    }
}
