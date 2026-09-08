// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions;

/// <summary>
/// EN: Handles current position projection queries.
/// FA: Query مربوط به Projection موقعیت‌های فعلی را مدیریت می‌کند.
/// </summary>
public sealed class GetPortfolioPositionsHandler
    : IRequestHandler<GetPortfolioPositionsQuery, Result<GetPortfolioPositionsResponse>>
{
    private readonly IPortfolioTransactionRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioPositionsHandler(IPortfolioTransactionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// EN: Builds current open positions from the immutable ledger.
    /// FA: موقعیت‌های باز فعلی را از دفتر تغییرناپذیر می‌سازد.
    /// </summary>
    public async Task<Result<GetPortfolioPositionsResponse>> Handle(
        GetPortfolioPositionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioPositionsResponse>.Fail(
                new Error("PortfolioTransaction.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        List<PortfolioTransaction> ledger =
            await _repository.GetLedgerAsync(portfolioId, null, cancellationToken);

        List<PortfolioPositionResponse> positions = [];

        foreach (IGrouping<string, PortfolioTransaction> group in
                 ledger.GroupBy(item => item.ListingId.Value.ToString()))
        {
            decimal quantity = 0m;
            decimal bookCost = 0m;
            string currencyId = string.Empty;

            foreach (PortfolioTransaction transaction in
                     group.OrderBy(item => item.ExecutedOn).ThenBy(item => item.Id.Value.ToString()))
            {
                currencyId = transaction.CurrencyId.Value.ToString();

                if (transaction.Type == PortfolioEventType.Buy)
                {
                    quantity += transaction.Quantity;
                    bookCost += transaction.GrossValue + transaction.TotalCosts;
                }
                else
                {
                    if (quantity <= 0m)
                        continue;

                    decimal averageCost = bookCost / quantity;
                    quantity -= transaction.Quantity;
                    bookCost -= averageCost * transaction.Quantity;

                    if (quantity == 0m)
                        bookCost = 0m;
                }
            }

            if (quantity > 0m)
            {
                positions.Add(
                    new PortfolioPositionResponse(
                        group.Key,
                        currencyId,
                        quantity,
                        bookCost / quantity,
                        bookCost));
            }
        }

        return Result<GetPortfolioPositionsResponse>.Success(
            new GetPortfolioPositionsResponse(
                positions.OrderBy(item => item.ListingId).ToList()));
    }
}
