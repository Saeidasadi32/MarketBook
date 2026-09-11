// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk;

/// <summary>
/// EN: One rolling historical VaR/CVaR point.
/// FA: یک نقطه Rolling مربوط به VaR/CVaR تاریخی.
/// </summary>
/// <param name="WindowFrom">EN: Beginning timestamp of the rolling return window. FA: زمان شروع پنجره بازده Rolling.</param>
/// <param name="WindowTo">EN: Ending timestamp of the rolling return window. FA: زمان پایان پنجره بازده Rolling.</param>
/// <param name="ObservationCount">EN: Number of periodic returns in the window. FA: تعداد بازده‌های دوره‌ای در پنجره.</param>
/// <param name="HistoricalQuantileReturn">EN: Raw historical quantile return. FA: صدک خام بازده تاریخی.</param>
/// <param name="ValueAtRiskReturn">EN: Positive VaR loss ratio. FA: نسبت زیان مثبت VaR.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive CVaR loss ratio. FA: نسبت زیان مثبت CVaR.</param>
/// <param name="TailObservationCount">EN: Number of observations included in the CVaR tail. FA: تعداد مشاهدات واردشده در دنباله CVaR.</param>
public sealed record PortfolioRollingValueAtRiskPointResponse(
    DateTimeOffset WindowFrom,
    DateTimeOffset WindowTo,
    int ObservationCount,
    decimal HistoricalQuantileReturn,
    decimal ValueAtRiskReturn,
    decimal ConditionalValueAtRiskReturn,
    int TailObservationCount);

/// <summary>
/// EN: Rolling historical VaR/CVaR response.
/// FA: پاسخ Rolling مربوط به VaR/CVaR تاریخی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when source periodic-return data is complete. FA: وقتی داده بازده دوره‌ای منبع کامل باشد true است.</param>
/// <param name="WindowPeriods">EN: Requested full rolling window size. FA: اندازه پنجره کامل Rolling درخواست‌شده.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
/// <param name="TailProbability">EN: One minus confidence level. FA: یک منهای سطح اطمینان.</param>
/// <param name="PointCount">EN: Number of full rolling points. FA: تعداد نقاط کامل Rolling.</param>
/// <param name="Points">EN: Ordered rolling VaR/CVaR points. FA: نقاط مرتب Rolling مربوط به VaR/CVaR.</param>
public sealed record GetPortfolioRollingValueAtRiskResponse(
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
    IReadOnlyCollection<PortfolioRollingValueAtRiskPointResponse> Points);
