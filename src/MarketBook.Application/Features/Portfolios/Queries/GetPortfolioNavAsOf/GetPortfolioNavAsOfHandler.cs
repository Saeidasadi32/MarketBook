// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf
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

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;

/// <summary>
/// EN: Reconstructs portfolio NAV from ledger entries and market prices available at a historical cutoff.
/// FA: NAV پرتفوی را از سطرهای Ledger و قیمت‌های بازار موجود در یک لحظه تاریخی بازسازی می‌کند.
/// </summary>
public sealed class GetPortfolioNavAsOfHandler
    : IRequestHandler<GetPortfolioNavAsOfQuery, Result<GetPortfolioNavAsOfResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;

    /// <summary>
    /// EN: Initializes the historical NAV handler.
    /// FA: Handler مربوط به NAV تاریخی را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioNavAsOfHandler(
        ISender sender,
        IPortfolioRepository portfolioRepository)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(portfolioRepository);

        _sender = sender;
        _portfolioRepository = portfolioRepository;
    }

    /// <summary>
    /// EN: Calculates historical NAV independently for every source currency.
    /// FA: NAV تاریخی را برای هر ارز مبدا به‌صورت مستقل محاسبه می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioNavAsOfResponse>> Handle(
        GetPortfolioNavAsOfQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioNavAsOfResponse>.Fail(
                new Error(
                    "PortfolioNavAsOf.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioNavAsOfResponse>.Fail(
                new Error(
                    "PortfolioNavAsOf.PortfolioNotFound",
                    "The portfolio was not found."));
        }

        Result<GetPortfolioCashBalancesResponse> cashResult =
            await _sender.Send(
                new GetPortfolioCashBalancesQuery(
                    request.PortfolioId,
                    request.AsOf),
                cancellationToken);

        if (cashResult.IsFailure)
        {
            return Result<GetPortfolioNavAsOfResponse>.Fail(cashResult.Error);
        }

        Result<GetPortfolioValuationResponse> valuationResult =
            await _sender.Send(
                new GetPortfolioValuationQuery(
                    request.PortfolioId,
                    null,
                    request.AsOf),
                cancellationToken);

        if (valuationResult.IsFailure)
        {
            return Result<GetPortfolioNavAsOfResponse>.Fail(
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

        List<PortfolioNavAsOfCurrencyResponse> currencies = [];

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
                new PortfolioNavAsOfCurrencyResponse(
                    currencyId,
                    cashBalance,
                    pricedMarketValue,
                    pricedNetAssetValue,
                    isComplete,
                    netAssetValue,
                    pricedPositionCount,
                    unpricedPositionCount));
        }

        return Result<GetPortfolioNavAsOfResponse>.Success(
            new GetPortfolioNavAsOfResponse(
                portfolio.Id.Value.ToString(),
                request.AsOf,
                currencies));
    }

    private static Error MapValuationError(Error error)
        => error.Code switch
        {
            "PortfolioValuation.MixedCurrencyCostBasis" =>
                new Error("PortfolioNavAsOf.MixedCurrencyCostBasis", error.Message),
            "PortfolioValuation.ListingNotFound" =>
                new Error("PortfolioNavAsOf.ListingNotFound", error.Message),
            "PortfolioValuation.QuoteCurrencyChanged" =>
                new Error("PortfolioNavAsOf.QuoteCurrencyChanged", error.Message),
            _ => error
        };
}
