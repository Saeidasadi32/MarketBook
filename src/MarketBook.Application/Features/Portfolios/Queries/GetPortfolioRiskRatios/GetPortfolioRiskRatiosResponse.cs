// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios;

/// <summary>
/// EN: Portfolio Sharpe and Sortino ratio projection.
/// FA: Projection نسبت‌های Sharpe و Sortino پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: Mirrors completeness of DOC-0033 risk statistics. FA: کامل‌بودن آمار ریسک DOC-0033 را منعکس می‌کند.</param>
/// <param name="ObservationCount">EN: Number of periodic-return observations. FA: تعداد مشاهدات بازده دوره‌ای.</param>
/// <param name="AnnualizationPeriodsPerYear">EN: Annualization periods inherited from DOC-0033. FA: تعداد دوره سالانه‌سازی دریافت‌شده از DOC-0033.</param>
/// <param name="RiskFreeRateAnnual">EN: Requested annual arithmetic risk-free rate. FA: نرخ بدون‌ریسک حسابی سالانه درخواست‌شده.</param>
/// <param name="RiskFreeRatePeriodic">EN: Annual risk-free rate divided by periods per year. FA: نرخ بدون‌ریسک سالانه تقسیم بر تعداد دوره‌های سال.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Requested annual arithmetic minimum acceptable return. FA: حداقل بازده قابل‌قبول حسابی سالانه درخواست‌شده.</param>
/// <param name="MinimumAcceptableReturnPeriodic">EN: Annual MAR divided by periods per year. FA: MAR سالانه تقسیم بر تعداد دوره‌های سال.</param>
/// <param name="IsSharpeCalculable">EN: True when Sharpe has sufficient data and positive volatility. FA: وقتی Sharpe داده کافی و نوسان مثبت داشته باشد true است.</param>
/// <param name="SharpeRatio">EN: Annualized Sharpe ratio using the requested risk-free rate. FA: نسبت Sharpe سالانه‌شده با نرخ بدون‌ریسک درخواست‌شده.</param>
/// <param name="IsSortinoCalculable">EN: True when Sortino has sufficient data and positive MAR-relative downside deviation. FA: وقتی Sortino داده کافی و انحراف نزولی مثبت نسبت به MAR داشته باشد true است.</param>
/// <param name="SortinoRatio">EN: Annualized Sortino ratio using the requested MAR. FA: نسبت Sortino سالانه‌شده با MAR درخواست‌شده.</param>
/// <param name="MeanPeriodicReturn">EN: Arithmetic mean periodic return from DOC-0033. FA: میانگین حسابی بازده دوره‌ای از DOC-0033.</param>
/// <param name="MeanPeriodicExcessReturnOverRiskFree">EN: Mean periodic return minus periodic risk-free rate. FA: میانگین بازده دوره‌ای منهای نرخ بدون‌ریسک دوره‌ای.</param>
/// <param name="MeanPeriodicExcessReturnOverMinimumAcceptableReturn">EN: Mean periodic return minus periodic MAR. FA: میانگین بازده دوره‌ای منهای MAR دوره‌ای.</param>
/// <param name="PeriodicVolatility">EN: Sample periodic volatility from DOC-0033. FA: نوسان نمونه دوره‌ای از DOC-0033.</param>
/// <param name="PeriodicDownsideDeviationRelativeToMinimumAcceptableReturn">EN: Downside deviation recalculated against the requested periodic MAR. FA: انحراف نزولی بازمحاسبه‌شده نسبت به MAR دوره‌ای درخواست‌شده.</param>
public sealed record GetPortfolioRiskRatiosResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    int ObservationCount,
    int AnnualizationPeriodsPerYear,
    decimal RiskFreeRateAnnual,
    decimal RiskFreeRatePeriodic,
    decimal MinimumAcceptableReturnAnnual,
    decimal MinimumAcceptableReturnPeriodic,
    bool IsSharpeCalculable,
    decimal? SharpeRatio,
    bool IsSortinoCalculable,
    decimal? SortinoRatio,
    decimal? MeanPeriodicReturn,
    decimal? MeanPeriodicExcessReturnOverRiskFree,
    decimal? MeanPeriodicExcessReturnOverMinimumAcceptableReturn,
    decimal? PeriodicVolatility,
    decimal? PeriodicDownsideDeviationRelativeToMinimumAcceptableReturn);
