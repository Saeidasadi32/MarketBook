// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;

/// <summary>
/// EN: Translates historical per-currency NAV into the portfolio base currency using only FX rates eligible at the historical cutoff.
/// FA: NAV تاریخی به تفکیک ارز را فقط با نرخ‌های FX معتبر در لحظه تاریخی به ارز پایه پرتفوی ترجمه می‌کند.
/// </summary>
public sealed class GetPortfolioTranslatedNavAsOfHandler
    : IRequestHandler<GetPortfolioTranslatedNavAsOfQuery, Result<GetPortfolioTranslatedNavAsOfResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the historical translated NAV handler.
    /// FA: Handler مربوط به NAV تاریخی ترجمه‌شده را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to reuse historical NAV projection. FA: Sender مدیاتور برای استفاده مجدد از Projection تاریخی NAV.</param>
    /// <param name="portfolioRepository">EN: Portfolio repository. FA: مخزن پرتفوی.</param>
    /// <param name="dbContext">EN: Application database context used to read historical FX rates. FA: Context دیتابیس برای خواندن نرخ‌های تاریخی FX.</param>
    public GetPortfolioTranslatedNavAsOfHandler(
        ISender sender,
        IPortfolioRepository portfolioRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(portfolioRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _sender = sender;
        _portfolioRepository = portfolioRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Reconstructs historical source NAV and translates it using the latest FX rate on or before the as-of date.
    /// FA: NAV تاریخی مبدا را بازسازی کرده و با آخرین نرخ FX در تاریخ As-Of یا قبل از آن ترجمه می‌کند.
    /// </summary>
    /// <param name="request">EN: Historical translated NAV query. FA: درخواست NAV تاریخی ترجمه‌شده.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical base-currency NAV projection. FA: Projection تاریخی NAV در ارز پایه.</returns>
    public async Task<Result<GetPortfolioTranslatedNavAsOfResponse>> Handle(
        GetPortfolioTranslatedNavAsOfQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioTranslatedNavAsOfResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNavAsOf.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioTranslatedNavAsOfResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNavAsOf.PortfolioNotFound",
                    "The portfolio was not found."));
        }

        if (portfolio.BaseCurrencyId is null)
        {
            return Result<GetPortfolioTranslatedNavAsOfResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNavAsOf.BaseCurrencyNotConfigured",
                    "The portfolio base currency has not been configured."));
        }

        Result<GetPortfolioNavAsOfResponse> sourceResult =
            await _sender.Send(
                new GetPortfolioNavAsOfQuery(
                    request.PortfolioId,
                    request.AsOf),
                cancellationToken);

        if (sourceResult.IsFailure)
        {
            return Result<GetPortfolioTranslatedNavAsOfResponse>.Fail(
                sourceResult.Error);
        }

        CurrencyId baseCurrencyId = portfolio.BaseCurrencyId;
        DateOnly asOfDate = DateOnly.FromDateTime(request.AsOf.DateTime);
        List<PortfolioTranslatedNavAsOfCurrencyResponse> translated = [];

        foreach (PortfolioNavAsOfCurrencyResponse source in sourceResult.Value!.Currencies)
        {
            CurrencyId sourceCurrencyId = CurrencyId.Parse(source.CurrencyId);

            TranslationRate? translationRate =
                await FindTranslationRateAsync(
                    sourceCurrencyId,
                    baseCurrencyId,
                    asOfDate,
                    cancellationToken);

            if (translationRate is null)
            {
                translated.Add(
                    new PortfolioTranslatedNavAsOfCurrencyResponse(
                        source.CurrencyId,
                        source.CashBalance,
                        source.PricedMarketValue,
                        source.PricedNetAssetValue,
                        source.IsComplete,
                        source.NetAssetValue,
                        source.PricedPositionCount,
                        source.UnpricedPositionCount,
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null));

                continue;
            }

            decimal rate = translationRate.SourceToBaseRate;
            decimal cashBalanceBase = source.CashBalance * rate;
            decimal pricedMarketValueBase = source.PricedMarketValue * rate;
            decimal translatedPricedNetAssetValueBase =
                source.PricedNetAssetValue * rate;

            decimal? translatedNetAssetValueBase =
                source.NetAssetValue.HasValue
                    ? source.NetAssetValue.Value * rate
                    : null;

            translated.Add(
                new PortfolioTranslatedNavAsOfCurrencyResponse(
                    source.CurrencyId,
                    source.CashBalance,
                    source.PricedMarketValue,
                    source.PricedNetAssetValue,
                    source.IsComplete,
                    source.NetAssetValue,
                    source.PricedPositionCount,
                    source.UnpricedPositionCount,
                    true,
                    translationRate.RateDate,
                    rate,
                    cashBalanceBase,
                    pricedMarketValueBase,
                    translatedPricedNetAssetValueBase,
                    translatedNetAssetValueBase));
        }

        bool isPricedNavFullyTranslated =
            translated.All(item => item.IsFxAvailable);

        bool isComplete =
            translated.All(
                item =>
                    item.IsFxAvailable &&
                    item.SourceIsComplete);

        decimal? pricedNetAssetValueBase =
            isPricedNavFullyTranslated
                ? translated.Sum(item => item.PricedNetAssetValueBase ?? 0m)
                : null;

        decimal? netAssetValueBase =
            isComplete
                ? translated.Sum(item => item.NetAssetValueBase ?? 0m)
                : null;

        return Result<GetPortfolioTranslatedNavAsOfResponse>.Success(
            new GetPortfolioTranslatedNavAsOfResponse(
                portfolio.Id.Value.ToString(),
                baseCurrencyId.Value.ToString(),
                request.AsOf,
                isPricedNavFullyTranslated,
                isComplete,
                pricedNetAssetValueBase,
                netAssetValueBase,
                translated
                    .OrderBy(item => item.CurrencyId, StringComparer.Ordinal)
                    .ToList()));
    }

    private async Task<TranslationRate?> FindTranslationRateAsync(
        CurrencyId sourceCurrencyId,
        CurrencyId baseCurrencyId,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        if (sourceCurrencyId == baseCurrencyId)
        {
            return new TranslationRate(null, 1m);
        }

        FxRate? fxRate =
            await _dbContext.Set<FxRate>()
                .AsNoTracking()
                .Where(
                    item =>
                        item.RateDate <= asOfDate &&
                        ((item.BaseCurrencyId == sourceCurrencyId &&
                          item.QuoteCurrencyId == baseCurrencyId) ||
                         (item.BaseCurrencyId == baseCurrencyId &&
                          item.QuoteCurrencyId == sourceCurrencyId)))
                .OrderByDescending(item => item.RateDate)
                .ThenByDescending(item => item.CreatedOn)
                .FirstOrDefaultAsync(cancellationToken);

        if (fxRate is null)
        {
            return null;
        }

        decimal sourceToBaseRate =
            fxRate.BaseCurrencyId == sourceCurrencyId
                ? fxRate.Rate
                : 1m / fxRate.Rate;

        return new TranslationRate(
            fxRate.RateDate,
            sourceToBaseRate);
    }

    private sealed record TranslationRate(
        DateOnly? RateDate,
        decimal SourceToBaseRate);
}
