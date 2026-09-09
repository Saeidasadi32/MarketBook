// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav;

/// <summary>
/// EN: Translates current per-currency portfolio NAV into the configured base currency.
/// FA: NAV جاری پرتفوی به تفکیک ارز را به ارز پایه تنظیم‌شده ترجمه می‌کند.
/// </summary>
public sealed class GetPortfolioTranslatedNavHandler
    : IRequestHandler<GetPortfolioTranslatedNavQuery, Result<GetPortfolioTranslatedNavResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the translated NAV handler.
    /// FA: Handler مربوط به NAV ترجمه‌شده را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to reuse current NAV projection. FA: Sender مدیاتور برای استفاده مجدد از Projection جاری NAV.</param>
    /// <param name="portfolioRepository">EN: Portfolio repository. FA: مخزن پرتفوی.</param>
    /// <param name="dbContext">EN: Application database context used to read FX rates. FA: Context دیتابیس برنامه برای خواندن نرخ‌های FX.</param>
    public GetPortfolioTranslatedNavHandler(
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
    /// EN: Calculates current source NAV and translates it to portfolio base currency without directly mixing source currencies.
    /// FA: NAV جاری ارزهای مبدا را محاسبه کرده و بدون جمع مستقیم ارزهای متفاوت به ارز پایه پرتفوی ترجمه می‌کند.
    /// </summary>
    /// <param name="request">EN: Translated NAV query. FA: درخواست NAV ترجمه‌شده.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Base-currency NAV projection. FA: Projection مربوط به NAV در ارز پایه.</returns>
    public async Task<Result<GetPortfolioTranslatedNavResponse>> Handle(
        GetPortfolioTranslatedNavQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioTranslatedNavResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNav.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioTranslatedNavResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNav.PortfolioNotFound",
                    "The portfolio was not found."));
        }

        if (portfolio.BaseCurrencyId is null)
        {
            return Result<GetPortfolioTranslatedNavResponse>.Fail(
                new Error(
                    "PortfolioTranslatedNav.BaseCurrencyNotConfigured",
                    "The portfolio base currency has not been configured."));
        }

        Result<GetPortfolioNavResponse> sourceResult =
            await _sender.Send(
                new GetPortfolioNavQuery(request.PortfolioId),
                cancellationToken);

        if (sourceResult.IsFailure)
        {
            return Result<GetPortfolioTranslatedNavResponse>.Fail(
                sourceResult.Error);
        }

        CurrencyId baseCurrencyId = portfolio.BaseCurrencyId;
        List<PortfolioTranslatedNavCurrencyResponse> translated = [];

        foreach (PortfolioNavCurrencyResponse source in sourceResult.Value!.Currencies)
        {
            CurrencyId sourceCurrencyId = CurrencyId.Parse(source.CurrencyId);

            TranslationRate? translationRate =
                await FindTranslationRateAsync(
                    sourceCurrencyId,
                    baseCurrencyId,
                    cancellationToken);

            if (translationRate is null)
            {
                translated.Add(
                    new PortfolioTranslatedNavCurrencyResponse(
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
                new PortfolioTranslatedNavCurrencyResponse(
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

        return Result<GetPortfolioTranslatedNavResponse>.Success(
            new GetPortfolioTranslatedNavResponse(
                portfolio.Id.Value.ToString(),
                baseCurrencyId.Value.ToString(),
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
                        (item.BaseCurrencyId == sourceCurrencyId &&
                         item.QuoteCurrencyId == baseCurrencyId) ||
                        (item.BaseCurrencyId == baseCurrencyId &&
                         item.QuoteCurrencyId == sourceCurrencyId))
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
