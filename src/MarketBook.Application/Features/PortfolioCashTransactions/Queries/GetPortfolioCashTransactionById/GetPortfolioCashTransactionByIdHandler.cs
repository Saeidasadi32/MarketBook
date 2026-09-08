// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById;

/// <summary>EN: Handles cash transaction lookup by id. FA: دریافت تراکنش نقدی بر اساس شناسه را مدیریت می‌کند.</summary>
public sealed class GetPortfolioCashTransactionByIdHandler
    : IRequestHandler<GetPortfolioCashTransactionByIdQuery, Result<GetPortfolioCashTransactionByIdResponse>>
{
    private readonly IPortfolioCashTransactionRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی اولیه می‌کند.</summary>
    public GetPortfolioCashTransactionByIdHandler(IPortfolioCashTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>EN: Gets one immutable ledger entry. FA: یک سطر تغییرناپذیر دفتر را دریافت می‌کند.</summary>
    public async Task<Result<GetPortfolioCashTransactionByIdResponse>> Handle(
        GetPortfolioCashTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioCashTransactionId.TryParse(request.Id, out PortfolioCashTransactionId? id) || id is null)
            return Result<GetPortfolioCashTransactionByIdResponse>.Fail(new Error(
                "PortfolioCashTransaction.InvalidId",
                "The cash transaction identifier is invalid."));

        PortfolioCashTransaction? item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return Result<GetPortfolioCashTransactionByIdResponse>.Fail(new Error(
                "PortfolioCashTransaction.NotFound",
                "The cash transaction was not found."));

        return Result<GetPortfolioCashTransactionByIdResponse>.Success(new(
            item.Id.Value.ToString(),
            item.PortfolioId.Value.ToString(),
            item.CurrencyId.Value.ToString(),
            (int)item.Type,
            item.Amount,
            item.SignedAmount,
            item.OccurredOn,
            item.ReferenceType,
            item.ReferenceId,
            item.Description,
            item.CreatedOn));
    }
}
