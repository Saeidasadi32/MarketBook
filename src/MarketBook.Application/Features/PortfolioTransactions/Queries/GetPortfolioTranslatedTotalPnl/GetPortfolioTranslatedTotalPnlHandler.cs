// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTotalPnl;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTranslatedTotalPnl;

/// <summary>
/// EN: Translates per-currency total P/L into the configured portfolio base currency.
/// FA: سود/زیان کل به تفکیک ارز را به ارز پایه تنظیم‌شده پرتفوی ترجمه می‌کند.
/// </summary>
public sealed class GetPortfolioTranslatedTotalPnlHandler
    : IRequestHandler<GetPortfolioTranslatedTotalPnlQuery, Result<GetPortfolioTranslatedTotalPnlResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the translated P/L handler.
    /// FA: Handler سود/زیان ترجمه‌شده را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioTranslatedTotalPnlHandler(
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
    /// EN: Calculates source-currency P/L and translates it without mixing currencies directly.
    /// FA: سود/زیان ارزهای مبدا را محاسبه کرده و بدون جمع مستقیم ارزهای مختلف آن را ترجمه می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioTranslatedTotalPnlResponse>> Handle(
        GetPortfolioTranslatedTotalPnlQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioTranslatedTotalPnlResponse>.Fail(
                new Error(
                    "PortfolioTranslatedPnl.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioTranslatedTotalPnlResponse>.Fail(
                new Error(
                    "PortfolioTranslatedPnl.PortfolioNotFound",
                    "The portfolio was not found."));
        }

        if (portfolio.BaseCurrencyId is null)
        {
            return Result<GetPortfolioTranslatedTotalPnlResponse>.Fail(
                new Error(
                    "PortfolioTranslatedPnl.BaseCurrencyNotConfigured",
                    "The portfolio base currency has not been configured."));
        }

        Result<GetPortfolioTotalPnlResponse> sourceResult =
            await _sender.Send(
                new GetPortfolioTotalPnlQuery(
                    request.PortfolioId,
                    request.ListingId),
                cancellationToken);

        if (sourceResult.IsFailure)
        {
            return Result<GetPortfolioTranslatedTotalPnlResponse>.Fail(
                sourceResult.Error);
        }

        CurrencyId baseCurrencyId = portfolio.BaseCurrencyId;
        List<PortfolioTranslatedPnlCurrencyResponse> translated = [];

        foreach (PortfolioTotalPnlCurrencyResponse source in sourceResult.Value!.Currencies)
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
                    new PortfolioTranslatedPnlCurrencyResponse(
                        source.CurrencyId,
                        source.IsFullyPriced,
                        source.RealizedProfitLoss,
                        source.UnrealizedProfitLoss,
                        source.TotalProfitLoss,
                        false,
                        null,
                        null,
                        null,
                        null,
                        null));

                continue;
            }

            decimal realizedBase =
                source.RealizedProfitLoss * translationRate.SourceToBaseRate;

            decimal? unrealizedBase =
                source.UnrealizedProfitLoss.HasValue
                    ? source.UnrealizedProfitLoss.Value * translationRate.SourceToBaseRate
                    : null;

            decimal? totalBase =
                source.TotalProfitLoss.HasValue
                    ? source.TotalProfitLoss.Value * translationRate.SourceToBaseRate
                    : null;

            translated.Add(
                new PortfolioTranslatedPnlCurrencyResponse(
                    source.CurrencyId,
                    source.IsFullyPriced,
                    source.RealizedProfitLoss,
                    source.UnrealizedProfitLoss,
                    source.TotalProfitLoss,
                    true,
                    translationRate.RateDate,
                    translationRate.SourceToBaseRate,
                    realizedBase,
                    unrealizedBase,
                    totalBase));
        }

        bool isRealizedFullyTranslated =
            translated.All(item => item.IsFxAvailable);

        bool isFullyPricedAndTranslated =
            translated.All(
                item =>
                    item.IsFxAvailable &&
                    item.IsFullyPriced);

        decimal? realizedTotal =
            isRealizedFullyTranslated
                ? translated.Sum(item => item.RealizedProfitLossBase ?? 0m)
                : null;

        decimal? unrealizedTotal =
            isFullyPricedAndTranslated
                ? translated.Sum(item => item.UnrealizedProfitLossBase ?? 0m)
                : null;

        decimal? totalProfitLoss =
            isFullyPricedAndTranslated
                ? translated.Sum(item => item.TotalProfitLossBase ?? 0m)
                : null;

        return Result<GetPortfolioTranslatedTotalPnlResponse>.Success(
            new GetPortfolioTranslatedTotalPnlResponse(
                portfolio.Id.Value.ToString(),
                baseCurrencyId.Value.ToString(),
                isRealizedFullyTranslated,
                isFullyPricedAndTranslated,
                realizedTotal,
                unrealizedTotal,
                totalProfitLoss,
                translated.OrderBy(item => item.CurrencyId).ToList()));
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
