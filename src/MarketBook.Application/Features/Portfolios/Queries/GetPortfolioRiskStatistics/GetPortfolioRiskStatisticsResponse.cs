// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;

/// <summary>
/// EN: One periodic TWR return reconstructed from adjacent wealth-index points.
/// FA: یک بازده دوره‌ای TWR که از دو نقطه متوالی شاخص ثروت بازسازی شده است.
/// </summary>
/// <param name="From">EN: Period beginning instant. FA: لحظه شروع دوره.</param>
/// <param name="To">EN: Period ending instant. FA: لحظه پایان دوره.</param>
/// <param name="Return">EN: Period return as a decimal ratio. FA: بازده دوره به‌صورت نسبت اعشاری.</param>
public sealed record PortfolioPeriodicReturnResponse(
    DateTimeOffset From,
    DateTimeOffset To,
    decimal Return);

/// <summary>
/// EN: Portfolio volatility and downside-deviation statistics.
/// FA: آمار نوسان و انحراف نزولی پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when every source performance point is complete. FA: وقتی همه نقاط منبع عملکرد کامل باشند true است.</param>
/// <param name="IsCalculable">EN: True when complete data provides at least two periodic-return observations. FA: وقتی داده کامل حداقل دو مشاهده بازده دوره‌ای فراهم کند true است.</param>
/// <param name="ObservationCount">EN: Number of periodic-return observations. FA: تعداد مشاهدات بازده دوره‌ای.</param>
/// <param name="AnnualizationPeriodsPerYear">EN: Calendar-based annualization periods: 365 Daily or 52 Weekly. FA: تعداد دوره تقویمی سالانه‌سازی: 365 برای Daily یا 52 برای Weekly.</param>
/// <param name="MeanPeriodicReturn">EN: Arithmetic mean periodic return. FA: میانگین حسابی بازده دوره‌ای.</param>
/// <param name="PeriodicVolatility">EN: Sample standard deviation using N-1. FA: انحراف معیار نمونه با مخرج N-1.</param>
/// <param name="AnnualizedVolatility">EN: Periodic volatility multiplied by square root of annualization periods. FA: نوسان دوره‌ای ضربدر ریشه تعداد دوره‌های سالانه‌سازی.</param>
/// <param name="PeriodicDownsideDeviation">EN: Zero-target downside deviation using all N observations. FA: انحراف نزولی با هدف صفر و استفاده از همه N مشاهده.</param>
/// <param name="AnnualizedDownsideDeviation">EN: Annualized zero-target downside deviation. FA: انحراف نزولی سالانه‌شده با هدف صفر.</param>
/// <param name="Returns">EN: Ordered periodic returns used by the statistics. FA: بازده‌های دوره‌ای مرتب استفاده‌شده در آمار.</param>
public sealed record GetPortfolioRiskStatisticsResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    bool IsCalculable,
    int ObservationCount,
    int AnnualizationPeriodsPerYear,
    decimal? MeanPeriodicReturn,
    decimal? PeriodicVolatility,
    decimal? AnnualizedVolatility,
    decimal? PeriodicDownsideDeviation,
    decimal? AnnualizedDownsideDeviation,
    IReadOnlyCollection<PortfolioPeriodicReturnResponse> Returns);
