// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison;

/// <summary>
/// EN: Combined historical return comparison for dashboard/reporting use.
/// FA: مقایسه ترکیبی بازده تاریخی برای استفاده در داشبورد و گزارش‌گیری.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning valuation instant. FA: لحظه ارزش‌گذاری ابتدای دوره.</param>
/// <param name="To">EN: Ending valuation instant. FA: لحظه ارزش‌گذاری انتهای دوره.</param>
/// <param name="IsDataComplete">EN: True when both TWR and XIRR have complete historical inputs. FA: وقتی ورودی تاریخی هر دو TWR و XIRR کامل باشد true است.</param>
/// <param name="AreBothReturnsAvailable">EN: True when both return measures are successfully calculable. FA: وقتی هر دو معیار بازده با موفقیت قابل محاسبه باشند true است.</param>
/// <param name="IsTwrComplete">EN: TWR historical data completeness flag. FA: پرچم کامل‌بودن داده تاریخی TWR.</param>
/// <param name="IsTwrCalculable">EN: TWR calculability flag. FA: پرچم قابل‌محاسبه بودن TWR.</param>
/// <param name="TimeWeightedReturn">EN: TWR decimal fraction. FA: بازده زمان‌وزن به‌صورت کسر اعشاری.</param>
/// <param name="ExternalFlowBoundaryCount">EN: Number of distinct external-flow boundaries used by TWR. FA: تعداد مرزهای متمایز جریان خارجی استفاده‌شده در TWR.</param>
/// <param name="IsXirrComplete">EN: XIRR historical data completeness flag. FA: پرچم کامل‌بودن داده تاریخی XIRR.</param>
/// <param name="HasValidXirrCashFlowSigns">EN: True when XIRR has at least one positive and one negative investor cash flow. FA: وقتی XIRR حداقل یک جریان مثبت و یک جریان منفی از دید سرمایه‌گذار داشته باشد true است.</param>
/// <param name="HasXirrSolution">EN: True when the XIRR solver found a supported root. FA: وقتی solver مربوط به XIRR یک ریشه پشتیبانی‌شده پیدا کرده باشد true است.</param>
/// <param name="MoneyWeightedReturn">EN: Annualized XIRR decimal fraction. FA: بازده پول‌وزن سالانه‌شده به‌صورت کسر اعشاری.</param>
/// <param name="XirrCashFlowCount">EN: Number of dated XIRR cash-flow entries. FA: تعداد جریان‌های نقدی تاریخ‌دار XIRR.</param>
/// <param name="MoneyWeightedMinusTimeWeighted">EN: XIRR minus TWR when both are available. FA: اختلاف XIRR و TWR وقتی هر دو در دسترس باشند.</param>
public sealed record GetPortfolioPerformanceComparisonResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    bool IsDataComplete,
    bool AreBothReturnsAvailable,
    bool IsTwrComplete,
    bool IsTwrCalculable,
    decimal? TimeWeightedReturn,
    int ExternalFlowBoundaryCount,
    bool IsXirrComplete,
    bool HasValidXirrCashFlowSigns,
    bool HasXirrSolution,
    decimal? MoneyWeightedReturn,
    int XirrCashFlowCount,
    decimal? MoneyWeightedMinusTimeWeighted);
