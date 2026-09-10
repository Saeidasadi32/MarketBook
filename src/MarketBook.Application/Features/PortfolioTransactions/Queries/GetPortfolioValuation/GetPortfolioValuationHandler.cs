// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation
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

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation;

/// <summary>
/// EN: Reconstructs open positions and values them with the latest available MarketPrice.
/// FA: موقعیت‌های باز را بازسازی کرده و با آخرین MarketPrice موجود ارزش‌گذاری می‌کند.
/// </summary>
public sealed class GetPortfolioValuationHandler
    : IRequestHandler<GetPortfolioValuationQuery, Result<GetPortfolioValuationResponse>>
{
    private readonly IPortfolioTransactionRepository _transactionRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IMarketPriceRepository _marketPriceRepository;

    /// <summary>
    /// EN: Initializes the portfolio valuation handler.
    /// FA: پردازشگر ارزش‌گذاری پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="transactionRepository">EN: Portfolio transaction repository. FA: مخزن تراکنش‌های پرتفوی.</param>
    /// <param name="listingRepository">EN: Listing repository. FA: مخزن لیستینگ.</param>
    /// <param name="marketPriceRepository">EN: Market-price repository. FA: مخزن قیمت بازار.</param>
    public GetPortfolioValuationHandler(
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
    /// EN: Handles current portfolio valuation and unrealized P/L calculation.
    /// FA: ارزش‌گذاری جاری پرتفوی و محاسبه سود/زیان تحقق‌نیافته را انجام می‌دهد.
    /// </summary>
    /// <param name="request">EN: Valuation query. FA: درخواست ارزش‌گذاری.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Portfolio valuation result. FA: نتیجه ارزش‌گذاری پرتفوی.</returns>
    public async Task<Result<GetPortfolioValuationResponse>> Handle(
        GetPortfolioValuationQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioValuationResponse>.Fail(
                new Error(
                    "PortfolioValuation.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        ListingId? listingId = null;

        if (!string.IsNullOrWhiteSpace(request.ListingId))
        {
            if (!ListingId.TryParse(request.ListingId, out ListingId? parsedListingId) ||
                parsedListingId is null)
            {
                return Result<GetPortfolioValuationResponse>.Fail(
                    new Error(
                        "PortfolioValuation.InvalidListingId",
                        "The listing identifier is invalid."));
            }

            listingId = parsedListingId;
        }

        List<PortfolioTransaction> ledger =
            request.AsOf.HasValue
                ? await _transactionRepository.GetLedgerAsync(
                    portfolioId,
                    listingId,
                    request.AsOf.Value,
                    cancellationToken)
                : await _transactionRepository.GetLedgerAsync(
                    portfolioId,
                    listingId,
                    cancellationToken);

        List<PortfolioValuationItemResponse> items = [];

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
                return Result<GetPortfolioValuationResponse>.Fail(
                    new Error(
                        "PortfolioValuation.MixedCurrencyCostBasis",
                        $"Listing '{group.Key}' contains portfolio transactions in multiple currencies."));
            }

            decimal quantity = 0m;
            decimal bookCost = 0m;

            foreach (PortfolioTransaction transaction in ordered)
            {
                if (transaction.Type == PortfolioEventType.Buy)
                {
                    quantity += transaction.Quantity;
                    bookCost += transaction.GrossValue + transaction.TotalCosts;
                    continue;
                }

                if (quantity <= 0m)
                {
                    continue;
                }

                decimal averageCost = bookCost / quantity;
                quantity -= transaction.Quantity;
                bookCost -= averageCost * transaction.Quantity;

                if (quantity == 0m)
                {
                    bookCost = 0m;
                }
            }

            if (quantity <= 0m)
            {
                continue;
            }

            ListingId currentListingId = ListingId.Parse(group.Key);
            Listing? listing =
                await _listingRepository.GetByIdAsync(
                    currentListingId,
                    cancellationToken);

            if (listing is null)
            {
                return Result<GetPortfolioValuationResponse>.Fail(
                    new Error(
                        "PortfolioValuation.ListingNotFound",
                        $"Listing '{group.Key}' was not found."));
            }

            string currencyId = currencies.Single();

            if (!string.Equals(
                    listing.QuoteCurrencyId.Value.ToString(),
                    currencyId,
                    StringComparison.Ordinal))
            {
                return Result<GetPortfolioValuationResponse>.Fail(
                    new Error(
                        "PortfolioValuation.QuoteCurrencyChanged",
                        $"Listing '{group.Key}' current quote currency differs from the transaction currency snapshot."));
            }

            MarketPrice? latestPrice =
                request.AsOf.HasValue
                    ? await _marketPriceRepository.GetLatestByListingIdAsync(
                        currentListingId,
                        DateOnly.FromDateTime(request.AsOf.Value.DateTime),
                        cancellationToken)
                    : await _marketPriceRepository.GetLatestByListingIdAsync(
                        currentListingId,
                        cancellationToken);

            decimal averageAcquisitionPrice = bookCost / quantity;

            if (latestPrice is null)
            {
                items.Add(
                    new PortfolioValuationItemResponse(
                        group.Key,
                        currencyId,
                        quantity,
                        averageAcquisitionPrice,
                        bookCost,
                        false,
                        null,
                        null,
                        null,
                        null,
                        null));

                continue;
            }

            decimal marketValue = quantity * latestPrice.LastPrice;
            decimal unrealizedProfitLoss = marketValue - bookCost;
            decimal? unrealizedReturnPercent =
                bookCost > 0m
                    ? unrealizedProfitLoss / bookCost * 100m
                    : null;

            items.Add(
                new PortfolioValuationItemResponse(
                    group.Key,
                    currencyId,
                    quantity,
                    averageAcquisitionPrice,
                    bookCost,
                    true,
                    latestPrice.TradingDate,
                    latestPrice.LastPrice,
                    marketValue,
                    unrealizedProfitLoss,
                    unrealizedReturnPercent));
        }

        return Result<GetPortfolioValuationResponse>.Success(
            new GetPortfolioValuationResponse(
                items.OrderBy(item => item.ListingId).ToList()));
    }
}
