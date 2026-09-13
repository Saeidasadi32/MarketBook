// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary;

/// <summary>
/// EN: Compact volatility and downside-deviation dashboard section.
/// FA: بخش فشرده نوسان و انحراف نزولی داشبورد.
/// </summary>
/// <param name="IsComplete">EN: Source completeness. FA: کامل‌بودن منبع.</param>
/// <param name="IsCalculable">EN: True when statistics are calculable. FA: وقتی آمار قابل محاسبه باشد true است.</param>
/// <param name="ObservationCount">EN: Periodic-return observation count. FA: تعداد مشاهدات بازده دوره‌ای.</param>
/// <param name="AnnualizationPeriodsPerYear">EN: Annualization periods. FA: تعداد دوره‌های سالانه‌سازی.</param>
/// <param name="MeanPeriodicReturn">EN: Arithmetic mean periodic return. FA: میانگین حسابی بازده دوره‌ای.</param>
/// <param name="AnnualizedVolatility">EN: Annualized volatility. FA: نوسان سالانه‌شده.</param>
/// <param name="AnnualizedDownsideDeviation">EN: Annualized zero-target downside deviation. FA: انحراف نزولی سالانه‌شده با هدف صفر.</param>
public sealed record PortfolioRiskStatisticsSummaryResponse(
    bool IsComplete,
    bool IsCalculable,
    int ObservationCount,
    int AnnualizationPeriodsPerYear,
    decimal? MeanPeriodicReturn,
    decimal? AnnualizedVolatility,
    decimal? AnnualizedDownsideDeviation);

/// <summary>
/// EN: Compact Sharpe and Sortino dashboard section.
/// FA: بخش فشرده Sharpe و Sortino داشبورد.
/// </summary>
/// <param name="IsComplete">EN: Source completeness. FA: کامل‌بودن منبع.</param>
/// <param name="RiskFreeRateAnnual">EN: Requested annual risk-free rate. FA: نرخ بدون‌ریسک سالانه درخواست‌شده.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Requested annual MAR. FA: MAR سالانه درخواست‌شده.</param>
/// <param name="IsSharpeCalculable">EN: True when Sharpe is calculable. FA: وقتی Sharpe قابل محاسبه باشد true است.</param>
/// <param name="SharpeRatio">EN: Annualized Sharpe ratio. FA: نسبت Sharpe سالانه‌شده.</param>
/// <param name="IsSortinoCalculable">EN: True when Sortino is calculable. FA: وقتی Sortino قابل محاسبه باشد true است.</param>
/// <param name="SortinoRatio">EN: Annualized Sortino ratio. FA: نسبت Sortino سالانه‌شده.</param>
public sealed record PortfolioRiskRatiosSummaryResponse(
    bool IsComplete,
    decimal RiskFreeRateAnnual,
    decimal MinimumAcceptableReturnAnnual,
    bool IsSharpeCalculable,
    decimal? SharpeRatio,
    bool IsSortinoCalculable,
    decimal? SortinoRatio);

/// <summary>
/// EN: Compact historical VaR/CVaR dashboard section.
/// FA: بخش فشرده VaR/CVaR تاریخی داشبورد.
/// </summary>
/// <param name="IsComplete">EN: Source completeness. FA: کامل‌بودن منبع.</param>
/// <param name="IsCalculable">EN: True when VaR/CVaR amounts are calculable. FA: وقتی مبالغ VaR/CVaR قابل محاسبه باشند true است.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
/// <param name="ObservationCount">EN: Return observation count. FA: تعداد مشاهدات بازده.</param>
/// <param name="TailObservationCount">EN: CVaR-tail observation count. FA: تعداد مشاهدات دنباله CVaR.</param>
/// <param name="NetAssetValueBase">EN: Historical NAV at To. FA: NAV تاریخی در To.</param>
/// <param name="ValueAtRiskReturn">EN: Positive VaR loss ratio. FA: نسبت زیان مثبت VaR.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive CVaR loss ratio. FA: نسبت زیان مثبت CVaR.</param>
/// <param name="ValueAtRiskAmountBase">EN: VaR amount in base currency. FA: مبلغ VaR در ارز پایه.</param>
/// <param name="ConditionalValueAtRiskAmountBase">EN: CVaR amount in base currency. FA: مبلغ CVaR در ارز پایه.</param>
public sealed record PortfolioValueAtRiskSummaryResponse(
    bool IsComplete,
    bool IsCalculable,
    decimal ConfidenceLevel,
    int ObservationCount,
    int TailObservationCount,
    decimal? NetAssetValueBase,
    decimal? ValueAtRiskReturn,
    decimal? ConditionalValueAtRiskReturn,
    decimal? ValueAtRiskAmountBase,
    decimal? ConditionalValueAtRiskAmountBase);

/// <summary>
/// EN: Compact relative and monetary drawdown dashboard section.
/// FA: بخش فشرده افت نسبی و مبلغی داشبورد.
/// </summary>
/// <param name="IsComplete">EN: Source completeness. FA: کامل‌بودن منبع.</param>
/// <param name="CurrentDrawdown">EN: Relative drawdown at To. FA: افت نسبی در To.</param>
/// <param name="CurrentDrawdownAmountBase">EN: Monetary drawdown at To. FA: مبلغ افت در To.</param>
/// <param name="MaximumDrawdown">EN: Most negative relative drawdown. FA: منفی‌ترین افت نسبی.</param>
/// <param name="MaximumDrawdownAmountBase">EN: Monetary amount at the maximum-relative-drawdown trough. FA: مبلغ افت در کف بیشینه افت نسبی.</param>
/// <param name="MaximumDrawdownPeakTimestamp">EN: Peak preceding maximum drawdown. FA: قله پیش از بیشینه افت.</param>
/// <param name="MaximumDrawdownTroughTimestamp">EN: Maximum-drawdown trough. FA: کف بیشینه افت.</param>
public sealed record PortfolioDrawdownSummaryResponse(
    bool IsComplete,
    decimal? CurrentDrawdown,
    decimal? CurrentDrawdownAmountBase,
    decimal? MaximumDrawdown,
    decimal? MaximumDrawdownAmountBase,
    DateTimeOffset? MaximumDrawdownPeakTimestamp,
    DateTimeOffset? MaximumDrawdownTroughTimestamp);

/// <summary>
/// EN: Compact drawdown-episode dashboard section.
/// FA: بخش فشرده دوره‌های افت داشبورد.
/// </summary>
/// <param name="IsComplete">EN: Source completeness. FA: کامل‌بودن منبع.</param>
/// <param name="EpisodeCount">EN: Number of drawdown episodes. FA: تعداد دوره‌های افت.</param>
/// <param name="HasActiveDrawdown">EN: True when a drawdown is active at To. FA: وقتی در To افت فعال باشد true است.</param>
/// <param name="MaximumEpisodeTroughDrawdownAmountBase">EN: Monetary trough loss of the deepest relative episode. FA: زیان مبلغی کف عمیق‌ترین دوره نسبی.</param>
/// <param name="MaximumEpisodeDurationDays">EN: Total duration of the deepest relative episode. FA: مدت کل عمیق‌ترین دوره نسبی.</param>
/// <param name="LongestEpisodeDurationDays">EN: Total duration of the longest episode. FA: مدت کل طولانی‌ترین دوره.</param>
/// <param name="ActiveEpisodeCurrentDrawdownAmountBase">EN: Current monetary loss of the active episode. FA: زیان مبلغی جاری دوره فعال.</param>
public sealed record PortfolioDrawdownEpisodesSummaryResponse(
    bool IsComplete,
    int EpisodeCount,
    bool HasActiveDrawdown,
    decimal? MaximumEpisodeTroughDrawdownAmountBase,
    decimal? MaximumEpisodeDurationDays,
    decimal? LongestEpisodeDurationDays,
    decimal? ActiveEpisodeCurrentDrawdownAmountBase);

/// <summary>
/// EN: Compact portfolio risk dashboard composed from existing analytics.
/// FA: داشبورد فشرده ریسک پرتفوی که از تحلیل‌های موجود ترکیب شده است.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency. FA: ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when every composed dashboard section is complete. FA: وقتی همه بخش‌های ترکیبی داشبورد کامل باشند true است.</param>
/// <param name="RiskStatistics">EN: Volatility/downside section. FA: بخش نوسان/ریسک نزولی.</param>
/// <param name="RiskRatios">EN: Sharpe/Sortino section. FA: بخش Sharpe/Sortino.</param>
/// <param name="ValueAtRisk">EN: Historical VaR/CVaR section. FA: بخش VaR/CVaR تاریخی.</param>
/// <param name="Drawdown">EN: Relative and monetary drawdown section. FA: بخش افت نسبی و مبلغی.</param>
/// <param name="DrawdownEpisodes">EN: Monetary drawdown episode section. FA: بخش دوره‌های افت مبلغی.</param>
public sealed record GetPortfolioRiskSummaryResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    PortfolioRiskStatisticsSummaryResponse RiskStatistics,
    PortfolioRiskRatiosSummaryResponse RiskRatios,
    PortfolioValueAtRiskSummaryResponse ValueAtRisk,
    PortfolioDrawdownSummaryResponse Drawdown,
    PortfolioDrawdownEpisodesSummaryResponse DrawdownEpisodes);
