// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;

/// <summary>
/// EN: Historical VaR and CVaR response.
/// FA: پاسخ VaR و CVaR تاریخی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when source periodic-return data is complete. FA: وقتی داده بازده دوره‌ای منبع کامل باشد true است.</param>
/// <param name="IsCalculable">EN: True when enough complete observations exist. FA: وقتی مشاهدات کامل کافی وجود داشته باشد true است.</param>
/// <param name="ObservationCount">EN: Number of periodic returns. FA: تعداد بازده‌های دوره‌ای.</param>
/// <param name="ConfidenceLevel">EN: Requested historical confidence level. FA: سطح اطمینان تاریخی درخواست‌شده.</param>
/// <param name="TailProbability">EN: One minus confidence level. FA: یک منهای سطح اطمینان.</param>
/// <param name="ValueAtRiskReturn">EN: Positive loss ratio at the historical quantile. FA: نسبت زیان مثبت در صدک تاریخی.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive average loss ratio of observations at or beyond VaR loss threshold. FA: میانگین نسبت زیان مثبت مشاهدات در آستانه VaR یا بدتر از آن.</param>
/// <param name="HistoricalQuantileReturn">EN: Raw return quantile used by VaR; normally negative in a loss tail. FA: صدک خام بازده مورد استفاده VaR که معمولاً در دنباله زیان منفی است.</param>
/// <param name="TailObservationCount">EN: Number of returns included in CVaR tail. FA: تعداد بازده‌های واردشده در دنباله CVaR.</param>
/// <param name="SortedReturns">EN: Ascending returns used for auditability. FA: بازده‌های صعودی مرتب‌شده برای قابلیت ممیزی.</param>
public sealed record GetPortfolioValueAtRiskResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    bool IsCalculable,
    int ObservationCount,
    decimal ConfidenceLevel,
    decimal TailProbability,
    decimal? ValueAtRiskReturn,
    decimal? ConditionalValueAtRiskReturn,
    decimal? HistoricalQuantileReturn,
    int TailObservationCount,
    IReadOnlyCollection<decimal> SortedReturns);
