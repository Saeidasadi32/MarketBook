// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits;

/// <summary>
/// EN: Evaluates optional request-scoped portfolio risk limits against DOC-0043.
/// FA: حدود اختیاری request-scoped ریسک پرتفوی را در برابر DOC-0043 ارزیابی می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval. FA: فاصله نمونه‌برداری.</param>
/// <param name="ConfidenceLevel">EN: VaR/CVaR confidence level. FA: سطح اطمینان VaR/CVaR.</param>
/// <param name="RiskFreeRateAnnual">EN: Annual risk-free rate for Sharpe. FA: نرخ بدون‌ریسک سالانه برای Sharpe.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual MAR for Sortino. FA: MAR سالانه برای Sortino.</param>
/// <param name="MaxAnnualizedVolatility">EN: Optional maximum annualized volatility. FA: حداکثر اختیاری نوسان سالانه‌شده.</param>
/// <param name="MaxValueAtRiskReturn">EN: Optional maximum VaR loss ratio. FA: حداکثر اختیاری نسبت زیان VaR.</param>
/// <param name="MaxValueAtRiskAmountBase">EN: Optional maximum VaR amount in base currency. FA: حداکثر اختیاری مبلغ VaR در ارز پایه.</param>
/// <param name="MaxDrawdownLossRatio">EN: Optional maximum positive drawdown-loss magnitude. FA: حداکثر اختیاری بزرگی مثبت زیان Drawdown.</param>
/// <param name="MaxDrawdownAmountBase">EN: Optional maximum monetary drawdown amount. FA: حداکثر اختیاری مبلغ Drawdown.</param>
/// <param name="MinSharpeRatio">EN: Optional minimum Sharpe ratio. FA: حداقل اختیاری نسبت Sharpe.</param>
/// <param name="MinSortinoRatio">EN: Optional minimum Sortino ratio. FA: حداقل اختیاری نسبت Sortino.</param>
public sealed record EvaluatePortfolioRiskLimitsQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    decimal ConfidenceLevel = 0.95m,
    decimal RiskFreeRateAnnual = 0m,
    decimal MinimumAcceptableReturnAnnual = 0m,
    decimal? MaxAnnualizedVolatility = null,
    decimal? MaxValueAtRiskReturn = null,
    decimal? MaxValueAtRiskAmountBase = null,
    decimal? MaxDrawdownLossRatio = null,
    decimal? MaxDrawdownAmountBase = null,
    decimal? MinSharpeRatio = null,
    decimal? MinSortinoRatio = null)
    : IRequest<Result<EvaluatePortfolioRiskLimitsResponse>>;
