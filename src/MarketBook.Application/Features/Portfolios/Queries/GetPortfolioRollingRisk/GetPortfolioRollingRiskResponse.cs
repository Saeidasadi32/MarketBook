// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk;

/// <summary>
/// EN: One rolling risk analytics point.
/// FA: یک نقطه تحلیل ریسک Rolling.
/// </summary>
/// <param name="WindowFrom">EN: Beginning timestamp of the rolling return window. FA: زمان شروع پنجره بازده Rolling.</param>
/// <param name="WindowTo">EN: Ending timestamp of the rolling return window. FA: زمان پایان پنجره بازده Rolling.</param>
/// <param name="ObservationCount">EN: Number of periodic-return observations in the window. FA: تعداد مشاهدات بازده دوره‌ای در پنجره.</param>
/// <param name="MeanPeriodicReturn">EN: Arithmetic mean periodic return. FA: میانگین حسابی بازده دوره‌ای.</param>
/// <param name="PeriodicVolatility">EN: Sample standard deviation using N-1. FA: انحراف معیار نمونه با مخرج N-1.</param>
/// <param name="AnnualizedVolatility">EN: Annualized volatility. FA: نوسان سالانه‌شده.</param>
/// <param name="PeriodicDownsideDeviation">EN: Downside deviation relative to configured MAR. FA: انحراف نزولی نسبت به MAR تنظیم‌شده.</param>
/// <param name="AnnualizedDownsideDeviation">EN: Annualized downside deviation. FA: انحراف نزولی سالانه‌شده.</param>
/// <param name="SharpeRatio">EN: Annualized Sharpe ratio; null when volatility is zero. FA: نسبت Sharpe سالانه‌شده؛ در نوسان صفر null است.</param>
/// <param name="SortinoRatio">EN: Annualized Sortino ratio; null when downside deviation is zero. FA: نسبت Sortino سالانه‌شده؛ در انحراف نزولی صفر null است.</param>
public sealed record PortfolioRollingRiskPointResponse(
    DateTimeOffset WindowFrom,
    DateTimeOffset WindowTo,
    int ObservationCount,
    decimal MeanPeriodicReturn,
    decimal PeriodicVolatility,
    decimal AnnualizedVolatility,
    decimal PeriodicDownsideDeviation,
    decimal AnnualizedDownsideDeviation,
    decimal? SharpeRatio,
    decimal? SortinoRatio);

/// <summary>
/// EN: Rolling portfolio risk analytics response.
/// FA: پاسخ تحلیل ریسک Rolling پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when the source risk statistics are complete. FA: وقتی آمار ریسک منبع کامل باشد true است.</param>
/// <param name="WindowPeriods">EN: Requested rolling window size in periodic returns. FA: اندازه پنجره Rolling بر حسب بازده‌های دوره‌ای.</param>
/// <param name="AnnualizationPeriodsPerYear">EN: Annualization cadence inherited from DOC-0033. FA: cadence سالانه‌سازی دریافت‌شده از DOC-0033.</param>
/// <param name="RiskFreeRateAnnual">EN: Annual arithmetic risk-free rate. FA: نرخ بدون‌ریسک حسابی سالانه.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual arithmetic MAR. FA: MAR حسابی سالانه.</param>
/// <param name="PointCount">EN: Number of rolling analytics points. FA: تعداد نقاط تحلیل Rolling.</param>
/// <param name="Points">EN: Ordered rolling risk analytics points. FA: نقاط مرتب تحلیل ریسک Rolling.</param>
public sealed record GetPortfolioRollingRiskResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    int WindowPeriods,
    int AnnualizationPeriodsPerYear,
    decimal RiskFreeRateAnnual,
    decimal MinimumAcceptableReturnAnnual,
    int PointCount,
    IReadOnlyCollection<PortfolioRollingRiskPointResponse> Points);
