// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets;

/// <summary>
/// EN: One standard performance-period result.
/// FA: نتیجه یک بازه استاندارد عملکرد.
/// </summary>
/// <param name="Preset">EN: Stable preset code: 1M, 3M, 6M, YTD, 1Y, or SinceInception. FA: کد پایدار بازه.</param>
/// <param name="From">EN: Resolved beginning instant. FA: لحظه شروع resolve‌شده.</param>
/// <param name="To">EN: Common ending instant. FA: لحظه پایان مشترک.</param>
/// <param name="IsDataComplete">EN: True when historical inputs for both TWR and XIRR are complete. FA: وقتی ورودی تاریخی هر دو TWR و XIRR کامل باشد true است.</param>
/// <param name="AreBothReturnsAvailable">EN: True when both TWR and XIRR are available. FA: وقتی هر دو TWR و XIRR در دسترس باشند true است.</param>
/// <param name="TimeWeightedReturn">EN: TWR decimal fraction. FA: TWR به‌صورت کسر اعشاری.</param>
/// <param name="MoneyWeightedReturn">EN: Annualized XIRR decimal fraction. FA: XIRR سالانه‌شده به‌صورت کسر اعشاری.</param>
/// <param name="MoneyWeightedMinusTimeWeighted">EN: XIRR minus TWR when both are available. FA: اختلاف XIRR و TWR وقتی هر دو در دسترس باشند.</param>
public sealed record PortfolioPerformancePresetResponse(
    string Preset,
    DateTimeOffset From,
    DateTimeOffset To,
    bool IsDataComplete,
    bool AreBothReturnsAvailable,
    decimal? TimeWeightedReturn,
    decimal? MoneyWeightedReturn,
    decimal? MoneyWeightedMinusTimeWeighted);

/// <summary>
/// EN: Standard portfolio performance periods for dashboard/reporting use.
/// FA: دوره‌های استاندارد عملکرد پرتفوی برای داشبورد و گزارش‌گیری.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="AsOf">EN: Common ending instant. FA: لحظه پایان مشترک.</param>
/// <param name="Periods">EN: Deterministically ordered preset results. FA: نتایج بازه‌ها با ترتیب قطعی.</param>
public sealed record GetPortfolioPerformancePresetsResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset AsOf,
    IReadOnlyCollection<PortfolioPerformancePresetResponse> Periods);
