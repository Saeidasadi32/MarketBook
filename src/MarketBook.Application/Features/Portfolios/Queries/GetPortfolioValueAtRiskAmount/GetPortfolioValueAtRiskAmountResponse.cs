// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount;

/// <summary>
/// EN: Historical VaR/CVaR ratios and their base-currency amount equivalents.
/// FA: نسبت‌های VaR/CVaR تاریخی و معادل مبلغی آن‌ها در ارز پایه.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant of the return history. FA: لحظه شروع تاریخچه بازده.</param>
/// <param name="To">EN: Ending instant and NAV valuation cutoff. FA: لحظه پایان و نقطه برش ارزش‌گذاری NAV.</param>
/// <param name="Interval">EN: Resolved return sampling interval. FA: فاصله نمونه‌برداری resolve‌شده بازده.</param>
/// <param name="ConfidenceLevel">EN: Historical confidence level. FA: سطح اطمینان تاریخی.</param>
/// <param name="IsComplete">EN: True when both return-risk data and translated historical NAV are complete. FA: وقتی هم داده ریسک بازده و هم NAV تاریخی ترجمه‌شده کامل باشند true است.</param>
/// <param name="IsCalculable">EN: True when risk ratios are calculable and ending NAV is strictly positive. FA: وقتی نسبت‌های ریسک قابل محاسبه و NAV پایانی بزرگ‌تر از صفر باشد true است.</param>
/// <param name="ObservationCount">EN: Number of periodic returns used by historical simulation. FA: تعداد بازده‌های دوره‌ای استفاده‌شده در شبیه‌سازی تاریخی.</param>
/// <param name="TailObservationCount">EN: Number of observations in the CVaR tail. FA: تعداد مشاهدات موجود در دنباله CVaR.</param>
/// <param name="NetAssetValueBase">EN: Complete NAV at To in portfolio base currency, or null when incomplete. FA: NAV کامل در لحظه To و در ارز پایه، یا null در صورت ناقص‌بودن.</param>
/// <param name="ValueAtRiskReturn">EN: Positive historical VaR loss ratio. FA: نسبت زیان مثبت VaR تاریخی.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive historical CVaR loss ratio. FA: نسبت زیان مثبت CVaR تاریخی.</param>
/// <param name="ValueAtRiskAmountBase">EN: VaR amount in portfolio base currency. FA: مبلغ VaR در ارز پایه پرتفوی.</param>
/// <param name="ConditionalValueAtRiskAmountBase">EN: CVaR amount in portfolio base currency. FA: مبلغ CVaR در ارز پایه پرتفوی.</param>
public sealed record GetPortfolioValueAtRiskAmountResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    decimal ConfidenceLevel,
    bool IsComplete,
    bool IsCalculable,
    int ObservationCount,
    int TailObservationCount,
    decimal? NetAssetValueBase,
    decimal? ValueAtRiskReturn,
    decimal? ConditionalValueAtRiskReturn,
    decimal? ValueAtRiskAmountBase,
    decimal? ConditionalValueAtRiskAmountBase);
