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
/// <param name="RiskFreeRateAnnual">EN: Annual risk-free rate used by Sharpe; fixed at zero in this foundation. FA: نرخ بدون‌ریسک سالانه Sharpe که در این نسخه صفر ثابت است.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual minimum acceptable return used by Sortino; fixed at zero in this foundation. FA: حداقل بازده قابل‌قبول سالانه Sortino که در این نسخه صفر ثابت است.</param>
/// <param name="IsSharpeCalculable">EN: True when Sharpe has sufficient data and positive volatility. FA: وقتی Sharpe داده کافی و نوسان مثبت داشته باشد true است.</param>
/// <param name="SharpeRatio">EN: Annualized zero-risk-free Sharpe ratio. FA: نسبت Sharpe سالانه‌شده با نرخ بدون‌ریسک صفر.</param>
/// <param name="IsSortinoCalculable">EN: True when Sortino has sufficient data and positive downside deviation. FA: وقتی Sortino داده کافی و انحراف نزولی مثبت داشته باشد true است.</param>
/// <param name="SortinoRatio">EN: Annualized zero-target Sortino ratio. FA: نسبت Sortino سالانه‌شده با هدف صفر.</param>
/// <param name="MeanPeriodicReturn">EN: Arithmetic mean periodic return from DOC-0033. FA: میانگین حسابی بازده دوره‌ای از DOC-0033.</param>
/// <param name="PeriodicVolatility">EN: Sample periodic volatility from DOC-0033. FA: نوسان نمونه دوره‌ای از DOC-0033.</param>
/// <param name="PeriodicDownsideDeviation">EN: Zero-target periodic downside deviation from DOC-0033. FA: انحراف نزولی دوره‌ای با هدف صفر از DOC-0033.</param>
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
    decimal MinimumAcceptableReturnAnnual,
    bool IsSharpeCalculable,
    decimal? SharpeRatio,
    bool IsSortinoCalculable,
    decimal? SortinoRatio,
    decimal? MeanPeriodicReturn,
    decimal? PeriodicVolatility,
    decimal? PeriodicDownsideDeviation);
