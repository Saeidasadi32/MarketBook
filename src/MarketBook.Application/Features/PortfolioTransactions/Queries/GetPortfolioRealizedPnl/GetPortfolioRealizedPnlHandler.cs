// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl;

/// <summary>
/// EN: Reconstructs realized P/L from the immutable portfolio transaction ledger.
/// FA: سود/زیان تحقق‌یافته را از دفتر تغییرناپذیر تراکنش‌های پرتفوی بازسازی می‌کند.
/// </summary>
public sealed class GetPortfolioRealizedPnlHandler
    : IRequestHandler<GetPortfolioRealizedPnlQuery, Result<GetPortfolioRealizedPnlResponse>>
{
    private readonly IPortfolioTransactionRepository _repository;

    /// <summary>
    /// EN: Initializes the realized P/L query handler.
    /// FA: پردازشگر پرس‌وجوی سود/زیان تحقق‌یافته را مقداردهی می‌کند.
    /// </summary>
    /// <param name="repository">EN: Portfolio transaction repository. FA: مخزن تراکنش‌های پرتفوی.</param>
    public GetPortfolioRealizedPnlHandler(IPortfolioTransactionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles realized P/L reconstruction using the requested cost-basis method.
    /// FA: بازسازی سود/زیان تحقق‌یافته را با روش بهای تمام‌شده درخواستی انجام می‌دهد.
    /// </summary>
    /// <param name="request">EN: Query request. FA: درخواست پرس‌وجو.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Realized P/L projection result. FA: نتیجه تصویر سود/زیان تحقق‌یافته.</returns>
    public async Task<Result<GetPortfolioRealizedPnlResponse>> Handle(
        GetPortfolioRealizedPnlQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioRealizedPnlResponse>.Fail(
                new Error(
                    "PortfolioTransaction.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        ListingId? listingId = null;

        if (!string.IsNullOrWhiteSpace(request.ListingId))
        {
            if (!ListingId.TryParse(request.ListingId, out ListingId? parsedListingId) ||
                parsedListingId is null)
            {
                return Result<GetPortfolioRealizedPnlResponse>.Fail(
                    new Error(
                        "PortfolioTransaction.InvalidListingId",
                        "The listing identifier is invalid."));
            }

            listingId = parsedListingId;
        }

        if (request.CostBasisMethod != (int)PortfolioCostBasisMethod.WeightedAverage)
        {
            return Result<GetPortfolioRealizedPnlResponse>.Fail(
                new Error(
                    "PortfolioTransaction.UnsupportedCostBasisMethod",
                    "Only WeightedAverage cost basis is supported in this slice."));
        }

        List<PortfolioTransaction> ledger =
            await _repository.GetLedgerAsync(
                portfolioId,
                listingId,
                cancellationToken);

        List<PortfolioRealizedPnlItemResponse> items = [];

        foreach (IGrouping<string, PortfolioTransaction> group in
                 ledger.GroupBy(item => item.ListingId.Value.ToString()))
        {
            List<PortfolioTransaction> ordered =
                group.OrderBy(item => item.ExecutedOn)
                    .ThenBy(item => item.Id.Value.ToString())
                    .ToList();

            string[] currencies =
                ordered.Select(item => item.CurrencyId.Value.ToString())
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();

            if (currencies.Length > 1)
            {
                return Result<GetPortfolioRealizedPnlResponse>.Fail(
                    new Error(
                        "PortfolioTransaction.MixedCurrencyCostBasis",
                        $"Listing '{group.Key}' contains transactions in multiple currencies. " +
                        "FX translation is required before realized P/L can be calculated safely."));
            }

            decimal openQuantity = 0m;
            decimal bookCost = 0m;
            decimal soldQuantity = 0m;
            decimal grossProceeds = 0m;
            decimal sellingCosts = 0m;
            decimal relievedCostBasis = 0m;
            decimal realizedProfitLoss = 0m;

            foreach (PortfolioTransaction transaction in ordered)
            {
                if (transaction.Type == PortfolioEventType.Buy)
                {
                    openQuantity += transaction.Quantity;
                    bookCost += transaction.GrossValue + transaction.TotalCosts;
                    continue;
                }

                if (openQuantity <= 0m)
                {
                    continue;
                }

                decimal averageCost = bookCost / openQuantity;
                decimal sellCostBasis = averageCost * transaction.Quantity;
                decimal netProceeds = transaction.GrossValue - transaction.TotalCosts;

                soldQuantity += transaction.Quantity;
                grossProceeds += transaction.GrossValue;
                sellingCosts += transaction.TotalCosts;
                relievedCostBasis += sellCostBasis;
                realizedProfitLoss += netProceeds - sellCostBasis;

                openQuantity -= transaction.Quantity;
                bookCost -= sellCostBasis;

                if (openQuantity == 0m)
                {
                    bookCost = 0m;
                }
            }

            decimal remainingAverageCost =
                openQuantity > 0m
                    ? bookCost / openQuantity
                    : 0m;

            items.Add(
                new PortfolioRealizedPnlItemResponse(
                    group.Key,
                    currencies.SingleOrDefault() ?? string.Empty,
                    soldQuantity,
                    grossProceeds,
                    sellingCosts,
                    grossProceeds - sellingCosts,
                    relievedCostBasis,
                    realizedProfitLoss,
                    openQuantity,
                    bookCost,
                    remainingAverageCost));
        }

        return Result<GetPortfolioRealizedPnlResponse>.Success(
            new GetPortfolioRealizedPnlResponse(
                (int)PortfolioCostBasisMethod.WeightedAverage,
                items.OrderBy(item => item.ListingId).ToList()));
    }
}
