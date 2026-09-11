// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown;

/// <summary>
/// EN: One drawdown point derived from the cumulative TWR wealth index.
/// FA: یک نقطه افت سرمایه که از شاخص ثروت TWR تجمعی مشتق شده است.
/// </summary>
/// <param name="Timestamp">EN: Valuation instant. FA: لحظه ارزش‌گذاری.</param>
/// <param name="IsCalculable">EN: True when cumulative TWR is available. FA: وقتی TWR تجمعی موجود باشد true است.</param>
/// <param name="CumulativeTimeWeightedReturn">EN: Cumulative TWR from From. FA: TWR تجمعی از From.</param>
/// <param name="WealthIndex">EN: TWR wealth index equal to 1 + cumulative TWR. FA: شاخص ثروت TWR برابر با 1 + TWR تجمعی.</param>
/// <param name="RunningPeakWealthIndex">EN: Highest wealth index observed up to this point. FA: بالاترین شاخص ثروت مشاهده‌شده تا این نقطه.</param>
/// <param name="RunningPeakTimestamp">EN: Timestamp of the running peak. FA: زمان قله جاری.</param>
/// <param name="Drawdown">EN: WealthIndex / RunningPeakWealthIndex - 1. FA: شاخص ثروت تقسیم بر قله جاری منهای یک.</param>
public sealed record PortfolioDrawdownPointResponse(
    DateTimeOffset Timestamp,
    bool IsCalculable,
    decimal? CumulativeTimeWeightedReturn,
    decimal? WealthIndex,
    decimal? RunningPeakWealthIndex,
    DateTimeOffset? RunningPeakTimestamp,
    decimal? Drawdown);

/// <summary>
/// EN: Historical portfolio drawdown analytics based on the cash-flow-neutral TWR wealth index.
/// FA: تحلیل تاریخی افت سرمایه پرتفوی بر پایه شاخص ثروت TWR که نسبت به جریان سرمایه خارجی خنثی است.
/// </summary>
public sealed record GetPortfolioDrawdownResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    decimal? PeakWealthIndex,
    DateTimeOffset? PeakTimestamp,
    decimal? CurrentDrawdown,
    decimal? MaximumDrawdown,
    DateTimeOffset? MaximumDrawdownPeakTimestamp,
    DateTimeOffset? MaximumDrawdownTroughTimestamp,
    IReadOnlyCollection<PortfolioDrawdownPointResponse> Points);
