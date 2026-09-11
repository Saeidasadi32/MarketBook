// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRiskAmount
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRiskAmount;

/// <summary>
/// EN: One rolling historical VaR/CVaR point with base-currency monetary exposure.
/// FA: یک نقطه VaR/CVaR تاریخی Rolling همراه با exposure مبلغی در ارز پایه.
/// </summary>
/// <param name="WindowFrom">EN: First periodic-return timestamp in the rolling window. FA: زمان نخستین بازده دوره‌ای در پنجره Rolling.</param>
/// <param name="WindowTo">EN: Last periodic-return timestamp and NAV valuation cutoff. FA: زمان آخرین بازده دوره‌ای و نقطه برش NAV.</param>
/// <param name="ObservationCount">EN: Number of periodic returns in the window. FA: تعداد بازده‌های دوره‌ای در پنجره.</param>
/// <param name="TailObservationCount">EN: Number of CVaR tail observations. FA: تعداد مشاهدات دنباله CVaR.</param>
/// <param name="IsComplete">EN: True when the historical NAV at WindowTo is complete. FA: وقتی NAV تاریخی در WindowTo کامل باشد true است.</param>
/// <param name="IsCalculable">EN: True when the point has complete strictly-positive NAV and calculable ratios. FA: وقتی نقطه NAV کامل و مثبت و نسبت‌های قابل محاسبه داشته باشد true است.</param>
/// <param name="NetAssetValueBase">EN: Historical NAV at WindowTo in base currency. FA: NAV تاریخی در WindowTo به ارز پایه.</param>
/// <param name="HistoricalQuantileReturn">EN: Raw historical quantile return. FA: صدک خام بازده تاریخی.</param>
/// <param name="ValueAtRiskReturn">EN: Positive VaR loss ratio. FA: نسبت زیان مثبت VaR.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive CVaR loss ratio. FA: نسبت زیان مثبت CVaR.</param>
/// <param name="ValueAtRiskAmountBase">EN: VaR amount in base currency. FA: مبلغ VaR در ارز پایه.</param>
/// <param name="ConditionalValueAtRiskAmountBase">EN: CVaR amount in base currency. FA: مبلغ CVaR در ارز پایه.</param>
public sealed record PortfolioRollingValueAtRiskAmountPointResponse(
    DateTimeOffset WindowFrom,
    DateTimeOffset WindowTo,
    int ObservationCount,
    int TailObservationCount,
    bool IsComplete,
    bool IsCalculable,
    decimal? NetAssetValueBase,
    decimal HistoricalQuantileReturn,
    decimal ValueAtRiskReturn,
    decimal ConditionalValueAtRiskReturn,
    decimal? ValueAtRiskAmountBase,
    decimal? ConditionalValueAtRiskAmountBase);

/// <summary>
/// EN: Rolling historical VaR/CVaR amount analytics.
/// FA: تحلیل مبلغی VaR/CVaR تاریخی Rolling.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when the rolling-return source and every point NAV are complete. FA: وقتی منبع بازده Rolling و NAV همه نقاط کامل باشند true است.</param>
/// <param name="WindowPeriods">EN: Full rolling window size. FA: اندازه پنجره کامل Rolling.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
/// <param name="TailProbability">EN: One minus confidence level. FA: یک منهای سطح اطمینان.</param>
/// <param name="PointCount">EN: Number of rolling points. FA: تعداد نقاط Rolling.</param>
/// <param name="CalculablePointCount">EN: Number of points with calculable monetary VaR/CVaR. FA: تعداد نقاط دارای VaR/CVaR مبلغی قابل محاسبه.</param>
/// <param name="Points">EN: Ordered rolling monetary risk points. FA: نقاط مرتب ریسک مبلغی Rolling.</param>
public sealed record GetPortfolioRollingValueAtRiskAmountResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    int WindowPeriods,
    decimal ConfidenceLevel,
    decimal TailProbability,
    int PointCount,
    int CalculablePointCount,
    IReadOnlyCollection<PortfolioRollingValueAtRiskAmountPointResponse> Points);
