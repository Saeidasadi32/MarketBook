// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;

/// <summary>
/// EN: One historical performance chart point.
/// FA: یک نقطه از سری تاریخی عملکرد.
/// </summary>
/// <param name="Timestamp">EN: Point valuation instant. FA: لحظه ارزش‌گذاری نقطه.</param>
/// <param name="IsNavComplete">EN: True when translated NAV is complete. FA: وقتی NAV ترجمه‌شده کامل باشد true است.</param>
/// <param name="NetAssetValueBase">EN: Complete NAV in portfolio base currency. FA: NAV کامل در ارز پایه پرتفوی.</param>
/// <param name="IsTwrCalculable">EN: True when cumulative TWR is calculable. FA: وقتی TWR تجمعی قابل محاسبه باشد true است.</param>
/// <param name="CumulativeTimeWeightedReturn">EN: Cumulative TWR from From to this point. FA: TWR تجمعی از From تا این نقطه.</param>
public sealed record PortfolioPerformanceSeriesPointResponse(
    DateTimeOffset Timestamp,
    bool IsNavComplete,
    decimal? NetAssetValueBase,
    bool IsTwrCalculable,
    decimal? CumulativeTimeWeightedReturn);

/// <summary>
/// EN: One exact external capital-flow marker for chart overlays.
/// FA: یک marker دقیق جریان سرمایه خارجی برای نمایش روی نمودار.
/// </summary>
/// <param name="CashTransactionId">EN: Cash transaction identifier. FA: شناسه تراکنش نقدی.</param>
/// <param name="CurrencyId">EN: Source currency identifier. FA: شناسه ارز مبدأ.</param>
/// <param name="Type">EN: Deposit or Withdrawal. FA: نوع Deposit یا Withdrawal.</param>
/// <param name="OccurredOn">EN: Exact flow instant. FA: لحظه دقیق جریان.</param>
/// <param name="SignedSourceAmount">EN: Portfolio-perspective signed source amount. FA: مبلغ علامت‌دار ارز مبدأ از دید پرتفوی.</param>
/// <param name="IsFxAvailable">EN: True when historical FX translation is available. FA: وقتی FX تاریخی برای ترجمه موجود باشد true است.</param>
/// <param name="SignedAmountBase">EN: Portfolio-perspective signed amount in base currency. FA: مبلغ علامت‌دار در ارز پایه از دید پرتفوی.</param>
public sealed record PortfolioPerformanceSeriesFlowMarkerResponse(
    string CashTransactionId,
    string CurrencyId,
    string Type,
    DateTimeOffset OccurredOn,
    decimal SignedSourceAmount,
    bool IsFxAvailable,
    decimal? SignedAmountBase);

/// <summary>
/// EN: Historical dashboard chart-series projection.
/// FA: Projection سری تاریخی نمودار داشبورد.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Series beginning instant. FA: لحظه شروع سری.</param>
/// <param name="To">EN: Series ending instant. FA: لحظه پایان سری.</param>
/// <param name="Interval">EN: Resolved interval code. FA: کد فاصله resolve‌شده.</param>
/// <param name="IsComplete">EN: True when all chart points and external-flow translations are complete. FA: وقتی همه نقاط نمودار و ترجمه جریان‌های خارجی کامل باشند true است.</param>
/// <param name="Points">EN: Deterministically ordered chart points. FA: نقاط نمودار با ترتیب قطعی.</param>
/// <param name="ExternalFlows">EN: Exact external-flow markers. FA: markerهای دقیق جریان خارجی.</param>
public sealed record GetPortfolioPerformanceSeriesResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    IReadOnlyCollection<PortfolioPerformanceSeriesPointResponse> Points,
    IReadOnlyCollection<PortfolioPerformanceSeriesFlowMarkerResponse> ExternalFlows);
