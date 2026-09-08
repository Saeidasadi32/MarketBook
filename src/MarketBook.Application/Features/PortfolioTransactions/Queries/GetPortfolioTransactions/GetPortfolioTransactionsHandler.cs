// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions;

/// <summary>
/// EN: Handles paged portfolio transaction queries.
/// FA: Query صفحه‌بندی‌شده تراکنش‌های پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class GetPortfolioTransactionsHandler
    : IRequestHandler<GetPortfolioTransactionsQuery, Result<GetPortfolioTransactionsResponse>>
{
    private readonly IPortfolioTransactionRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioTransactionsHandler(IPortfolioTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the paged ledger query.
    /// FA: Query دفتر صفحه‌بندی‌شده را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioTransactionsResponse>> Handle(
        GetPortfolioTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioTransactionsResponse>.Fail(
                new Error("PortfolioTransaction.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<PortfolioTransaction> page =
            await _repository.GetPagedAsync(portfolioId, pageRequest, cancellationToken);

        List<PortfolioTransactionItemResponse> items = page.Items
            .Select(item => new PortfolioTransactionItemResponse(
                item.Id.Value.ToString(),
                item.ListingId.Value.ToString(),
                item.CurrencyId.Value.ToString(),
                (int)item.Type,
                item.Quantity,
                item.Price,
                item.TotalCosts,
                item.ExecutedOn))
            .ToList();

        return Result<GetPortfolioTransactionsResponse>.Success(
            new GetPortfolioTransactionsResponse(
                items,
                page.Page,
                page.PageSize,
                page.TotalCount));
    }
}
