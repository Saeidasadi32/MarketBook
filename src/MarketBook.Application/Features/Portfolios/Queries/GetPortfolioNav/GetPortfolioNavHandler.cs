// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioValuation;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;

/// <summary>
/// EN: Combines the immutable cash ledger with current open-position valuation to project NAV by currency.
/// FA: دفتر نقدی تغییرناپذیر را با ارزش‌گذاری جاری موقعیت‌های باز ترکیب کرده و NAV را به تفکیک ارز محاسبه می‌کند.
/// </summary>
public sealed class GetPortfolioNavHandler
    : IRequestHandler<GetPortfolioNavQuery, Result<GetPortfolioNavResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;

    /// <summary>
    /// EN: Initializes the portfolio NAV handler.
    /// FA: Handler محاسبه NAV پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to reuse existing projections. FA: Sender مدیاتور برای استفاده مجدد از Projectionهای موجود.</param>
    /// <param name="portfolioRepository">EN: Portfolio repository. FA: مخزن پرتفوی.</param>
    public GetPortfolioNavHandler(
        ISender sender,
        IPortfolioRepository portfolioRepository)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(portfolioRepository);

        _sender = sender;
        _portfolioRepository = portfolioRepository;
    }

    /// <summary>
    /// EN: Calculates current NAV independently for every currency.
    /// FA: NAV جاری را به‌صورت مستقل برای هر ارز محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: NAV query. FA: درخواست NAV.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Per-currency NAV result. FA: نتیجه NAV به تفکیک ارز.</returns>
    public async Task<Result<GetPortfolioNavResponse>> Handle(
        GetPortfolioNavQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioNavResponse>.Fail(
                new Error(
                    "PortfolioNav.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioNavResponse>.Fail(
                new Error(
                    "PortfolioNav.PortfolioNotFound",
                    "The portfolio was not found."));
        }

        Result<GetPortfolioCashBalancesResponse> cashResult =
            await _sender.Send(
                new GetPortfolioCashBalancesQuery(request.PortfolioId),
                cancellationToken);

        if (cashResult.IsFailure)
        {
            return Result<GetPortfolioNavResponse>.Fail(cashResult.Error);
        }

        Result<GetPortfolioValuationResponse> valuationResult =
            await _sender.Send(
                new GetPortfolioValuationQuery(request.PortfolioId, null),
                cancellationToken);

        if (valuationResult.IsFailure)
        {
            return Result<GetPortfolioNavResponse>.Fail(
                MapValuationError(valuationResult.Error));
        }

        Dictionary<string, decimal> cashByCurrency =
            cashResult.Value!.Items.ToDictionary(
                item => item.CurrencyId,
                item => item.Balance,
                StringComparer.Ordinal);

        Dictionary<string, List<PortfolioValuationItemResponse>> positionsByCurrency =
            valuationResult.Value!.Items
                .GroupBy(item => item.CurrencyId, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList(),
                    StringComparer.Ordinal);

        string[] currencyIds =
            cashByCurrency.Keys
                .Concat(positionsByCurrency.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(item => item, StringComparer.Ordinal)
                .ToArray();

        List<PortfolioNavCurrencyResponse> currencies = [];

        foreach (string currencyId in currencyIds)
        {
            decimal cashBalance =
                cashByCurrency.TryGetValue(currencyId, out decimal balance)
                    ? balance
                    : 0m;

            List<PortfolioValuationItemResponse> positions =
                positionsByCurrency.TryGetValue(
                    currencyId,
                    out List<PortfolioValuationItemResponse>? currencyPositions)
                    ? currencyPositions
                    : [];

            int pricedPositionCount = positions.Count(item => item.IsPriced);
            int unpricedPositionCount = positions.Count - pricedPositionCount;

            decimal pricedMarketValue =
                positions
                    .Where(item => item.IsPriced)
                    .Sum(item => item.MarketValue ?? 0m);

            decimal pricedNetAssetValue = cashBalance + pricedMarketValue;
            bool isComplete = unpricedPositionCount == 0;
            decimal? netAssetValue =
                isComplete
                    ? pricedNetAssetValue
                    : null;

            currencies.Add(
                new PortfolioNavCurrencyResponse(
                    currencyId,
                    cashBalance,
                    pricedMarketValue,
                    pricedNetAssetValue,
                    isComplete,
                    netAssetValue,
                    pricedPositionCount,
                    unpricedPositionCount));
        }

        return Result<GetPortfolioNavResponse>.Success(
            new GetPortfolioNavResponse(
                portfolio.Id.Value.ToString(),
                currencies));
    }

    private static Error MapValuationError(Error error)
        => error.Code switch
        {
            "PortfolioValuation.MixedCurrencyCostBasis" =>
                new Error("PortfolioNav.MixedCurrencyCostBasis", error.Message),
            "PortfolioValuation.ListingNotFound" =>
                new Error("PortfolioNav.ListingNotFound", error.Message),
            "PortfolioValuation.QuoteCurrencyChanged" =>
                new Error("PortfolioNav.QuoteCurrencyChanged", error.Message),
            _ => error
        };
}
