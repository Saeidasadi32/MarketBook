// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot;

/// <summary>
/// EN: Latest rolling volatility, downside, Sharpe, and Sortino snapshot.
/// FA: آخرین snapshot نوسان، ریسک نزولی، Sharpe و Sortino Rolling.
/// </summary>
/// <param name="IsComplete">EN: Source rolling-risk completeness. FA: کامل‌بودن منبع Rolling Risk.</param>
/// <param name="HasPoint">EN: True when at least one full rolling window exists. FA: وقتی حداقل یک پنجره کامل Rolling وجود داشته باشد true است.</param>
/// <param name="PointCount">EN: Total available rolling-risk point count. FA: تعداد کل نقاط Rolling Risk موجود.</param>
/// <param name="WindowFrom">EN: Latest rolling window beginning. FA: شروع آخرین پنجره Rolling.</param>
/// <param name="WindowTo">EN: Latest rolling window ending. FA: پایان آخرین پنجره Rolling.</param>
/// <param name="ObservationCount">EN: Latest-window return observations. FA: تعداد مشاهدات بازده در آخرین پنجره.</param>
/// <param name="MeanPeriodicReturn">EN: Latest-window arithmetic mean return. FA: میانگین حسابی بازده آخرین پنجره.</param>
/// <param name="AnnualizedVolatility">EN: Latest annualized volatility. FA: آخرین نوسان سالانه‌شده.</param>
/// <param name="AnnualizedDownsideDeviation">EN: Latest annualized downside deviation. FA: آخرین انحراف نزولی سالانه‌شده.</param>
/// <param name="SharpeRatio">EN: Latest annualized Sharpe ratio. FA: آخرین نسبت Sharpe سالانه‌شده.</param>
/// <param name="SortinoRatio">EN: Latest annualized Sortino ratio. FA: آخرین نسبت Sortino سالانه‌شده.</param>
public sealed record PortfolioRollingRiskLatestSnapshotResponse(
    bool IsComplete,
    bool HasPoint,
    int PointCount,
    DateTimeOffset? WindowFrom,
    DateTimeOffset? WindowTo,
    int? ObservationCount,
    decimal? MeanPeriodicReturn,
    decimal? AnnualizedVolatility,
    decimal? AnnualizedDownsideDeviation,
    decimal? SharpeRatio,
    decimal? SortinoRatio);

/// <summary>
/// EN: Latest rolling historical VaR/CVaR ratio and amount snapshot.
/// FA: آخرین snapshot نسبت و مبلغ VaR/CVaR تاریخی Rolling.
/// </summary>
/// <param name="IsComplete">EN: Source rolling monetary VaR completeness. FA: کامل‌بودن منبع Rolling VaR مبلغی.</param>
/// <param name="HasPoint">EN: True when at least one full rolling VaR window exists. FA: وقتی حداقل یک پنجره کامل Rolling VaR وجود داشته باشد true است.</param>
/// <param name="PointCount">EN: Total rolling monetary VaR point count. FA: تعداد کل نقاط Rolling VaR مبلغی.</param>
/// <param name="CalculablePointCount">EN: Total calculable monetary VaR point count. FA: تعداد کل نقاط قابل محاسبه Rolling VaR مبلغی.</param>
/// <param name="WindowFrom">EN: Latest rolling VaR window beginning. FA: شروع آخرین پنجره Rolling VaR.</param>
/// <param name="WindowTo">EN: Latest rolling VaR window ending and NAV cutoff. FA: پایان آخرین پنجره Rolling VaR و cutoff مربوط به NAV.</param>
/// <param name="ObservationCount">EN: Latest-window return observations. FA: تعداد مشاهدات بازده در آخرین پنجره.</param>
/// <param name="TailObservationCount">EN: Latest-window CVaR tail observations. FA: تعداد مشاهدات دنباله CVaR در آخرین پنجره.</param>
/// <param name="IsCalculable">EN: Latest-point monetary calculability. FA: قابل‌محاسبه‌بودن مبلغ در آخرین نقطه.</param>
/// <param name="NetAssetValueBase">EN: Historical NAV at latest WindowTo. FA: NAV تاریخی در آخرین WindowTo.</param>
/// <param name="ValueAtRiskReturn">EN: Latest positive VaR loss ratio. FA: آخرین نسبت زیان مثبت VaR.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Latest positive CVaR loss ratio. FA: آخرین نسبت زیان مثبت CVaR.</param>
/// <param name="ValueAtRiskAmountBase">EN: Latest VaR amount in base currency. FA: آخرین مبلغ VaR در ارز پایه.</param>
/// <param name="ConditionalValueAtRiskAmountBase">EN: Latest CVaR amount in base currency. FA: آخرین مبلغ CVaR در ارز پایه.</param>
public sealed record PortfolioRollingValueAtRiskLatestSnapshotResponse(
    bool IsComplete,
    bool HasPoint,
    int PointCount,
    int CalculablePointCount,
    DateTimeOffset? WindowFrom,
    DateTimeOffset? WindowTo,
    int? ObservationCount,
    int? TailObservationCount,
    bool IsCalculable,
    decimal? NetAssetValueBase,
    decimal? ValueAtRiskReturn,
    decimal? ConditionalValueAtRiskReturn,
    decimal? ValueAtRiskAmountBase,
    decimal? ConditionalValueAtRiskAmountBase);

/// <summary>
/// EN: Compact latest-point snapshot for rolling portfolio risk analytics.
/// FA: snapshot فشرده آخرین نقطه تحلیل‌های ریسک Rolling پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency. FA: ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved interval. FA: فاصله resolve‌شده.</param>
/// <param name="WindowPeriods">EN: Requested full rolling-window size. FA: اندازه پنجره کامل Rolling درخواست‌شده.</param>
/// <param name="ConfidenceLevel">EN: Requested historical confidence level. FA: سطح اطمینان تاریخی درخواست‌شده.</param>
/// <param name="RiskFreeRateAnnual">EN: Requested annual risk-free rate. FA: نرخ بدون‌ریسک سالانه درخواست‌شده.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Requested annual MAR. FA: MAR سالانه درخواست‌شده.</param>
/// <param name="IsComplete">EN: True when both composed rolling sources are complete. FA: وقتی هر دو منبع Rolling ترکیبی کامل باشند true است.</param>
/// <param name="RollingRisk">EN: Latest DOC-0036 rolling-risk snapshot. FA: آخرین snapshot ریسک Rolling از DOC-0036.</param>
/// <param name="RollingValueAtRisk">EN: Latest DOC-0040 rolling monetary VaR/CVaR snapshot. FA: آخرین snapshot Rolling VaR/CVaR مبلغی از DOC-0040.</param>
public sealed record GetPortfolioRollingRiskSummarySnapshotResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    int WindowPeriods,
    decimal ConfidenceLevel,
    decimal RiskFreeRateAnnual,
    decimal MinimumAcceptableReturnAnnual,
    bool IsComplete,
    PortfolioRollingRiskLatestSnapshotResponse RollingRisk,
    PortfolioRollingValueAtRiskLatestSnapshotResponse RollingValueAtRisk);
