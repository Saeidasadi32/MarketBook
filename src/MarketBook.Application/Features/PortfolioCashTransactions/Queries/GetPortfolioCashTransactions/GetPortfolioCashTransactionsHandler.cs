// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions;

/// <summary>EN: Handles paged cash-ledger queries. FA: Queryهای صفحه‌بندی‌شده دفتر نقدی را مدیریت می‌کند.</summary>
public sealed class GetPortfolioCashTransactionsHandler
    : IRequestHandler<GetPortfolioCashTransactionsQuery, Result<GetPortfolioCashTransactionsResponse>>
{
    private readonly IPortfolioCashTransactionRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی اولیه می‌کند.</summary>
    public GetPortfolioCashTransactionsHandler(IPortfolioCashTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>EN: Gets normalized paged cash-ledger entries. FA: سطرهای صفحه‌بندی‌شده و نرمال‌شده دفتر نقدی را دریافت می‌کند.</summary>
    public async Task<Result<GetPortfolioCashTransactionsResponse>> Handle(
        GetPortfolioCashTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
            return Result<GetPortfolioCashTransactionsResponse>.Fail(new Error(
                "PortfolioCashTransaction.InvalidPortfolioId",
                "The portfolio identifier is invalid."));

        CurrencyId? currencyId = null;
        if (!string.IsNullOrWhiteSpace(request.CurrencyId) &&
            (!CurrencyId.TryParse(request.CurrencyId, out currencyId) || currencyId is null))
        {
            return Result<GetPortfolioCashTransactionsResponse>.Fail(new Error(
                "PortfolioCashTransaction.InvalidCurrencyId",
                "The currency identifier is invalid."));
        }

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<PortfolioCashTransaction> page =
            await _repository.GetPagedAsync(portfolioId, currencyId, pageRequest, cancellationToken);

        List<PortfolioCashTransactionItemResponse> items = page.Items.Select(item => new PortfolioCashTransactionItemResponse(
            item.Id.Value.ToString(),
            item.CurrencyId.Value.ToString(),
            (int)item.Type,
            item.Amount,
            item.SignedAmount,
            item.OccurredOn,
            item.ReferenceType,
            item.ReferenceId,
            item.Description)).ToList();

        return Result<GetPortfolioCashTransactionsResponse>.Success(new(
            items,
            page.Page,
            page.PageSize,
            page.TotalCount));
    }
}
