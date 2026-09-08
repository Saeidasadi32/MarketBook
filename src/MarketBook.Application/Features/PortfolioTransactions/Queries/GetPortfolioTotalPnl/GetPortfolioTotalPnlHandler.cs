// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl;

/// <summary>
/// EN: Reconstructs realized and unrealized P/L from the immutable trade ledger.
/// FA: سود/زیان تحقق‌یافته و تحقق‌نیافته را از دفتر تغییرناپذیر معاملات بازسازی می‌کند.
/// </summary>
public sealed class GetPortfolioTotalPnlHandler
    : IRequestHandler<GetPortfolioTotalPnlQuery, Result<GetPortfolioTotalPnlResponse>>
{
    private readonly IPortfolioTransactionRepository _transactionRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IMarketPriceRepository _marketPriceRepository;

    /// <summary>
    /// EN: Initializes the total portfolio P/L handler.
    /// FA: پردازشگر سود/زیان کل پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="transactionRepository">EN: Portfolio transaction repository. FA: مخزن تراکنش‌های پرتفوی.</param>
    /// <param name="listingRepository">EN: Listing repository. FA: مخزن لیستینگ.</param>
    /// <param name="marketPriceRepository">EN: Market-price repository. FA: مخزن قیمت بازار.</param>
    public GetPortfolioTotalPnlHandler(
        IPortfolioTransactionRepository transactionRepository,
        IListingRepository listingRepository,
        IMarketPriceRepository marketPriceRepository)
    {
        ArgumentNullException.ThrowIfNull(transactionRepository);
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(marketPriceRepository);

        _transactionRepository = transactionRepository;
        _listingRepository = listingRepository;
        _marketPriceRepository = marketPriceRepository;
    }

    /// <summary>
    /// EN: Calculates realized, unrealized, and total P/L without cross-currency aggregation.
    /// FA: سود/زیان تحقق‌یافته، تحقق‌نیافته و کل را بدون تجمیع بین ارزها محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Total P/L query. FA: درخواست سود/زیان کل.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Portfolio P/L result. FA: نتیجه سود/زیان پرتفوی.</returns>
    public async Task<Result<GetPortfolioTotalPnlResponse>> Handle(
        GetPortfolioTotalPnlQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioTotalPnlResponse>.Fail(
                new Error(
                    "PortfolioTotalPnl.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        ListingId? listingId = null;

        if (!string.IsNullOrWhiteSpace(request.ListingId))
        {
            if (!ListingId.TryParse(request.ListingId, out ListingId? parsedListingId) ||
                parsedListingId is null)
            {
                return Result<GetPortfolioTotalPnlResponse>.Fail(
                    new Error(
                        "PortfolioTotalPnl.InvalidListingId",
                        "The listing identifier is invalid."));
            }

            listingId = parsedListingId;
        }

        List<PortfolioTransaction> ledger =
            await _transactionRepository.GetLedgerAsync(
                portfolioId,
                listingId,
                cancellationToken);

        List<PortfolioTotalPnlItemResponse> items = [];

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
                return Result<GetPortfolioTotalPnlResponse>.Fail(
                    new Error(
                        "PortfolioTotalPnl.MixedCurrencyCostBasis",
                        $"Listing '{group.Key}' contains portfolio transactions in multiple currencies."));
            }

            decimal openQuantity = 0m;
            decimal bookCost = 0m;
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
                decimal relievedCostBasis = averageCost * transaction.Quantity;
                decimal netProceeds = transaction.GrossValue - transaction.TotalCosts;

                realizedProfitLoss += netProceeds - relievedCostBasis;
                openQuantity -= transaction.Quantity;
                bookCost -= relievedCostBasis;

                if (openQuantity == 0m)
                {
                    bookCost = 0m;
                }
            }

            string currencyId = currencies.Single();
            ListingId currentListingId = ListingId.Parse(group.Key);

            if (openQuantity <= 0m)
            {
                items.Add(
                    new PortfolioTotalPnlItemResponse(
                        group.Key,
                        currencyId,
                        realizedProfitLoss,
                        0m,
                        0m,
                        true,
                        null,
                        0m,
                        0m,
                        realizedProfitLoss));

                continue;
            }

            Listing? listing =
                await _listingRepository.GetByIdAsync(
                    currentListingId,
                    cancellationToken);

            if (listing is null)
            {
                return Result<GetPortfolioTotalPnlResponse>.Fail(
                    new Error(
                        "PortfolioTotalPnl.ListingNotFound",
                        $"Listing '{group.Key}' was not found."));
            }

            if (!string.Equals(
                    listing.QuoteCurrencyId.Value.ToString(),
                    currencyId,
                    StringComparison.Ordinal))
            {
                return Result<GetPortfolioTotalPnlResponse>.Fail(
                    new Error(
                        "PortfolioTotalPnl.QuoteCurrencyChanged",
                        $"Listing '{group.Key}' current quote currency differs from the transaction currency snapshot."));
            }

            MarketPrice? latestPrice =
                await _marketPriceRepository.GetLatestByListingIdAsync(
                    currentListingId,
                    cancellationToken);

            if (latestPrice is null)
            {
                items.Add(
                    new PortfolioTotalPnlItemResponse(
                        group.Key,
                        currencyId,
                        realizedProfitLoss,
                        openQuantity,
                        bookCost,
                        false,
                        null,
                        null,
                        null,
                        null));

                continue;
            }

            decimal marketValue = openQuantity * latestPrice.LastPrice;
            decimal unrealizedProfitLoss = marketValue - bookCost;

            items.Add(
                new PortfolioTotalPnlItemResponse(
                    group.Key,
                    currencyId,
                    realizedProfitLoss,
                    openQuantity,
                    bookCost,
                    true,
                    latestPrice.LastPrice,
                    marketValue,
                    unrealizedProfitLoss,
                    realizedProfitLoss + unrealizedProfitLoss));
        }

        List<PortfolioTotalPnlCurrencyResponse> currenciesResponse = [];

        foreach (IGrouping<string, PortfolioTotalPnlItemResponse> currencyGroup in
                 items.GroupBy(item => item.CurrencyId))
        {
            bool isFullyPriced = currencyGroup.All(item => item.IsPriced);
            decimal realizedProfitLoss =
                currencyGroup.Sum(item => item.RealizedProfitLoss);

            decimal? unrealizedProfitLoss = isFullyPriced
                ? currencyGroup.Sum(item => item.UnrealizedProfitLoss ?? 0m)
                : null;

            decimal? totalProfitLoss = isFullyPriced
                ? realizedProfitLoss + unrealizedProfitLoss!.Value
                : null;

            currenciesResponse.Add(
                new PortfolioTotalPnlCurrencyResponse(
                    currencyGroup.Key,
                    isFullyPriced,
                    realizedProfitLoss,
                    unrealizedProfitLoss,
                    totalProfitLoss));
        }

        return Result<GetPortfolioTotalPnlResponse>.Success(
            new GetPortfolioTotalPnlResponse(
                currenciesResponse.OrderBy(item => item.CurrencyId).ToList(),
                items.OrderBy(item => item.CurrencyId)
                    .ThenBy(item => item.ListingId)
                    .ToList()));
    }
}
