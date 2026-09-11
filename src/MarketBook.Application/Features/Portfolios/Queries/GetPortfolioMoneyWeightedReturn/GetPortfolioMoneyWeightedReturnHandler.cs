// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn;

/// <summary>
/// EN: Builds a dated investor cash-flow stream and solves annualized money-weighted return using a bounded bisection XIRR solver.
/// FA: جریان نقدی تاریخ‌دار سرمایه‌گذار را می‌سازد و بازده پول‌وزن سالانه‌شده را با solver محدود و پایدار bisection برای XIRR حل می‌کند.
/// </summary>
public sealed class GetPortfolioMoneyWeightedReturnHandler
    : IRequestHandler<GetPortfolioMoneyWeightedReturnQuery, Result<GetPortfolioMoneyWeightedReturnResponse>>
{
    private const decimal MinimumRate = -0.9999m;
    private const decimal MaximumRate = 1000m;
    private const int MaximumIterations = 200;
    private const decimal FunctionTolerance = 0.00000001m;
    private const decimal RateTolerance = 0.0000000001m;

    private readonly ISender _sender;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the money-weighted return handler.
    /// FA: Handler بازده پول‌وزن را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    /// <param name="dbContext">EN: Application database context. FA: Context دیتابیس برنامه.</param>
    public GetPortfolioMoneyWeightedReturnHandler(
        ISender sender,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(dbContext);

        _sender = sender;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Creates the XIRR cash-flow stream and solves the annualized rate when possible.
    /// FA: جریان نقدی XIRR را ایجاد می‌کند و در صورت امکان نرخ سالانه‌شده را حل می‌کند.
    /// </summary>
    /// <param name="request">EN: XIRR request. FA: درخواست XIRR.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Money-weighted return projection. FA: Projection بازده پول‌وزن.</returns>
    public async Task<Result<GetPortfolioMoneyWeightedReturnResponse>> Handle(
        GetPortfolioMoneyWeightedReturnQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.From >= request.To)
        {
            return Result<GetPortfolioMoneyWeightedReturnResponse>.Fail(
                new Error(
                    "PortfolioXirr.InvalidPeriod",
                    "The XIRR period requires From to be earlier than To."));
        }

        Result<GetPortfolioPerformanceResponse> performanceResult =
            await _sender.Send(
                new GetPortfolioPerformanceQuery(
                    request.PortfolioId,
                    request.From,
                    request.To),
                cancellationToken);

        if (performanceResult.IsFailure)
        {
            return Result<GetPortfolioMoneyWeightedReturnResponse>.Fail(
                performanceResult.Error);
        }

        GetPortfolioPerformanceResponse performance = performanceResult.Value!;

        bool isComplete =
            performance.IsComplete &&
            performance.BeginningNetAssetValueBase.HasValue &&
            performance.EndingNetAssetValueBase.HasValue;

        List<PortfolioXirrCashFlowResponse> cashFlows = [];

        if (performance.BeginningNetAssetValueBase.HasValue)
        {
            cashFlows.Add(
                new PortfolioXirrCashFlowResponse(
                    request.From,
                    -performance.BeginningNetAssetValueBase.Value,
                    "BeginningNAV"));
        }

        if (isComplete)
        {
            PortfolioId portfolioId = PortfolioId.Parse(request.PortfolioId);

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

            Dictionary<string, PortfolioExternalCashFlowResponse> performanceFlows =
                performance.ExternalFlows.ToDictionary(
                    item => item.CashTransactionId,
                    StringComparer.Ordinal);

            foreach (PortfolioCashTransaction transaction in externalTransactions)
            {
                string transactionId = transaction.Id.Value.ToString();

                if (!performanceFlows.TryGetValue(
                        transactionId,
                        out PortfolioExternalCashFlowResponse? translatedFlow) ||
                    !translatedFlow.SignedAmountBase.HasValue)
                {
                    isComplete = false;
                    break;
                }

                decimal investorAmount =
                    translatedFlow.SignedAmountBase.Value > 0m
                        ? -translatedFlow.SignedAmountBase.Value
                        : -translatedFlow.SignedAmountBase.Value;

                string kind =
                    transaction.Type == PortfolioCashTransactionType.Deposit
                        ? "Deposit"
                        : "Withdrawal";

                cashFlows.Add(
                    new PortfolioXirrCashFlowResponse(
                        transaction.OccurredOn,
                        investorAmount,
                        kind));
            }
        }

        if (performance.EndingNetAssetValueBase.HasValue)
        {
            cashFlows.Add(
                new PortfolioXirrCashFlowResponse(
                    request.To,
                    performance.EndingNetAssetValueBase.Value,
                    "TerminalNAV"));
        }

        cashFlows =
            cashFlows
                .OrderBy(item => item.Date)
                .ThenBy(item => item.Kind, StringComparer.Ordinal)
                .ToList();

        bool hasNegative =
            cashFlows.Any(item => item.AmountBase < 0m);

        bool hasPositive =
            cashFlows.Any(item => item.AmountBase > 0m);

        bool hasValidCashFlowSigns =
            hasNegative &&
            hasPositive;

        decimal? annualizedMoneyWeightedReturn = null;
        bool hasSolution = false;

        if (isComplete && hasValidCashFlowSigns)
        {
            decimal? solvedRate = SolveXirr(cashFlows);

            if (solvedRate.HasValue)
            {
                annualizedMoneyWeightedReturn = solvedRate.Value;
                hasSolution = true;
            }
        }

        return Result<GetPortfolioMoneyWeightedReturnResponse>.Success(
            new GetPortfolioMoneyWeightedReturnResponse(
                request.PortfolioId,
                performance.BaseCurrencyId,
                request.From,
                request.To,
                isComplete,
                hasValidCashFlowSigns,
                hasSolution,
                annualizedMoneyWeightedReturn,
                cashFlows));
    }

    private static decimal? SolveXirr(
        IReadOnlyCollection<PortfolioXirrCashFlowResponse> cashFlows)
    {
        decimal lower = MinimumRate;
        decimal upper = MaximumRate;

        decimal lowerValue = EvaluateXnpv(cashFlows, lower);
        decimal upperValue = EvaluateXnpv(cashFlows, upper);

        if (Math.Abs(lowerValue) <= FunctionTolerance)
        {
            return lower;
        }

        if (Math.Abs(upperValue) <= FunctionTolerance)
        {
            return upper;
        }

        if (Math.Sign(lowerValue) == Math.Sign(upperValue))
        {
            return null;
        }

        for (int iteration = 0; iteration < MaximumIterations; iteration++)
        {
            decimal middle = (lower + upper) / 2m;
            decimal middleValue = EvaluateXnpv(cashFlows, middle);

            if (Math.Abs(middleValue) <= FunctionTolerance ||
                Math.Abs(upper - lower) <= RateTolerance)
            {
                return middle;
            }

            if (Math.Sign(lowerValue) == Math.Sign(middleValue))
            {
                lower = middle;
                lowerValue = middleValue;
            }
            else
            {
                upper = middle;
            }
        }

        return (lower + upper) / 2m;
    }

    private static decimal EvaluateXnpv(
        IReadOnlyCollection<PortfolioXirrCashFlowResponse> cashFlows,
        decimal rate)
    {
        PortfolioXirrCashFlowResponse first = cashFlows.OrderBy(item => item.Date).First();
        decimal total = 0m;

        foreach (PortfolioXirrCashFlowResponse cashFlow in cashFlows)
        {
            double days =
                (cashFlow.Date - first.Date).TotalDays;

            double years = days / 365.0d;
            double denominator =
                Math.Pow(
                    1.0d + (double)rate,
                    years);

            if (denominator == 0.0d ||
                double.IsNaN(denominator) ||
                double.IsInfinity(denominator))
            {
                return decimal.MaxValue;
            }

            total +=
                cashFlow.AmountBase /
                (decimal)denominator;
        }

        return total;
    }
}
