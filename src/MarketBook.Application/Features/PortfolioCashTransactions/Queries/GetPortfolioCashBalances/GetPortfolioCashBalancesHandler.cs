// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances;

/// <summary>EN: Rebuilds multi-currency cash balances from the immutable ledger. FA: موجودی نقدی چندارزی را از دفتر تغییرناپذیر بازسازی می‌کند.</summary>
public sealed class GetPortfolioCashBalancesHandler
    : IRequestHandler<GetPortfolioCashBalancesQuery, Result<GetPortfolioCashBalancesResponse>>
{
    private readonly IPortfolioCashTransactionRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی اولیه می‌کند.</summary>
    public GetPortfolioCashBalancesHandler(IPortfolioCashTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>EN: Projects balances grouped by currency. FA: موجودی‌ها را به تفکیک ارز محاسبه می‌کند.</summary>
    public async Task<Result<GetPortfolioCashBalancesResponse>> Handle(
        GetPortfolioCashBalancesQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
            return Result<GetPortfolioCashBalancesResponse>.Fail(new Error(
                "PortfolioCashTransaction.InvalidPortfolioId",
                "The portfolio identifier is invalid."));

        List<PortfolioCashTransaction> ledger = await _repository.GetLedgerAsync(portfolioId, cancellationToken);

        List<PortfolioCashBalanceItemResponse> items = ledger
            .GroupBy(item => item.CurrencyId)
            .Select(group => new PortfolioCashBalanceItemResponse(
                group.Key.Value.ToString(),
                group.Sum(item => item.SignedAmount)))
            .Where(item => item.Balance != 0m)
            .OrderBy(item => item.CurrencyId, StringComparer.Ordinal)
            .ToList();

        return Result<GetPortfolioCashBalancesResponse>.Success(new(items));
    }
}
