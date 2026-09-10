// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;

/// <summary>
/// EN: Builds a cash-flow-aware historical performance foundation without imposing TWR or money-weighted return semantics.
/// FA: مبنای عملکرد تاریخی را با لحاظ جریان نقدی خارجی و بدون تحمیل معنای TWR یا بازده پول‌وزن می‌سازد.
/// </summary>
public sealed class GetPortfolioPerformanceHandler
    : IRequestHandler<GetPortfolioPerformanceQuery, Result<GetPortfolioPerformanceResponse>>
{
    private readonly ISender _sender;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the portfolio performance handler.
    /// FA: Handler عملکرد پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to compose historical translated NAV projections. FA: Sender مدیاتور برای ترکیب Projectionهای NAV تاریخی ترجمه‌شده.</param>
    /// <param name="dbContext">EN: Application database context used for immutable cash ledger and FX reads. FA: Context دیتابیس برای خواندن دفتر نقدی تغییرناپذیر و نرخ‌های FX.</param>
    public GetPortfolioPerformanceHandler(
        ISender sender,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(dbContext);

        _sender = sender;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Computes boundary NAVs, classifies external flows, translates each flow at its occurrence date, and derives investment profit/loss.
    /// FA: NAVهای مرزی را محاسبه، جریان‌های خارجی را طبقه‌بندی، هر جریان را در تاریخ وقوع ترجمه و سود/زیان سرمایه‌گذاری را استخراج می‌کند.
    /// </summary>
    /// <param name="request">EN: Performance period request. FA: درخواست دوره عملکرد.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical performance foundation. FA: مبنای عملکرد تاریخی.</returns>
    public async Task<Result<GetPortfolioPerformanceResponse>> Handle(
        GetPortfolioPerformanceQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.From >= request.To)
        {
            return Result<GetPortfolioPerformanceResponse>.Fail(
                new Error(
                    "PortfolioPerformance.InvalidPeriod",
                    "The performance period requires From to be earlier than To."));
        }

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioPerformanceResponse>.Fail(
                new Error(
                    "PortfolioPerformance.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Result<GetPortfolioTranslatedNavAsOfResponse> beginningResult =
            await _sender.Send(
                new GetPortfolioTranslatedNavAsOfQuery(
                    request.PortfolioId,
                    request.From),
                cancellationToken);

        if (beginningResult.IsFailure)
        {
            return Result<GetPortfolioPerformanceResponse>.Fail(
                beginningResult.Error);
        }

        Result<GetPortfolioTranslatedNavAsOfResponse> endingResult =
            await _sender.Send(
                new GetPortfolioTranslatedNavAsOfQuery(
                    request.PortfolioId,
                    request.To),
                cancellationToken);

        if (endingResult.IsFailure)
        {
            return Result<GetPortfolioPerformanceResponse>.Fail(
                endingResult.Error);
        }

        GetPortfolioTranslatedNavAsOfResponse beginning = beginningResult.Value!;
        GetPortfolioTranslatedNavAsOfResponse ending = endingResult.Value!;

        if (!string.Equals(
                beginning.BaseCurrencyId,
                ending.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioPerformanceResponse>.Fail(
                new Error(
                    "PortfolioPerformance.BaseCurrencyMismatch",
                    "The beginning and ending projections do not use the same base currency."));
        }

        CurrencyId baseCurrencyId = CurrencyId.Parse(beginning.BaseCurrencyId);

        List<PortfolioCashTransaction> externalTransactions =
            await _dbContext.Set<PortfolioCashTransaction>()
                .AsNoTracking()
                .Where(
                    item =>
                        item.PortfolioId == portfolioId &&
                        item.OccurredOn > request.From &&
                        item.OccurredOn <= request.To &&
                        (item.Type == PortfolioCashTransactionType.Deposit ||
                         item.Type == PortfolioCashTransactionType.Withdrawal))
                .OrderBy(item => item.OccurredOn)
                .ThenBy(item => item.Id)
                .ToListAsync(cancellationToken);

        List<PortfolioExternalCashFlowResponse> externalFlows = [];

        foreach (PortfolioCashTransaction transaction in externalTransactions)
        {
            TranslationRate? translationRate =
                await FindTranslationRateAsync(
                    transaction.CurrencyId,
                    baseCurrencyId,
                    DateOnly.FromDateTime(transaction.OccurredOn.DateTime),
                    cancellationToken);

            decimal signedSourceAmount =
                transaction.Type == PortfolioCashTransactionType.Deposit
                    ? transaction.Amount
                    : -transaction.Amount;

            externalFlows.Add(
                new PortfolioExternalCashFlowResponse(
                    transaction.Id.Value.ToString(),
                    transaction.CurrencyId.Value.ToString(),
                    transaction.Type.ToString(),
                    transaction.OccurredOn,
                    signedSourceAmount,
                    translationRate is not null,
                    translationRate?.RateDate,
                    translationRate?.SourceToBaseRate,
                    translationRate is null
                        ? null
                        : signedSourceAmount * translationRate.SourceToBaseRate));
        }

        bool areExternalFlowsFullyTranslated =
            externalFlows.All(item => item.IsFxAvailable);

        decimal? netExternalFlowBase =
            areExternalFlowsFullyTranslated
                ? externalFlows.Sum(item => item.SignedAmountBase ?? 0m)
                : null;

        bool isBeginningNavComplete =
            beginning.IsComplete &&
            beginning.NetAssetValueBase.HasValue;

        bool isEndingNavComplete =
            ending.IsComplete &&
            ending.NetAssetValueBase.HasValue;

        bool isComplete =
            isBeginningNavComplete &&
            isEndingNavComplete &&
            areExternalFlowsFullyTranslated;

        decimal? investmentProfitLossBase =
            isComplete &&
            netExternalFlowBase.HasValue
                ? ending.NetAssetValueBase!.Value -
                  beginning.NetAssetValueBase!.Value -
                  netExternalFlowBase.Value
                : null;

        return Result<GetPortfolioPerformanceResponse>.Success(
            new GetPortfolioPerformanceResponse(
                request.PortfolioId,
                beginning.BaseCurrencyId,
                request.From,
                request.To,
                isBeginningNavComplete,
                isEndingNavComplete,
                areExternalFlowsFullyTranslated,
                isComplete,
                beginning.NetAssetValueBase,
                ending.NetAssetValueBase,
                netExternalFlowBase,
                investmentProfitLossBase,
                externalFlows));
    }

    private async Task<TranslationRate?> FindTranslationRateAsync(
        CurrencyId sourceCurrencyId,
        CurrencyId baseCurrencyId,
        DateOnly onOrBeforeDate,
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
                        item.RateDate <= onOrBeforeDate &&
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
