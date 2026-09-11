// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown;

/// <summary>
/// EN: One monetary drawdown point based on cash-flow-neutral TWR drawdown and historical base-currency NAV.
/// FA: یک نقطه افت مبلغی بر پایه Drawdown خنثی TWR و NAV تاریخی ارز پایه.
/// </summary>
/// <param name="Timestamp">EN: Valuation instant. FA: لحظه ارزش‌گذاری.</param>
/// <param name="IsComplete">EN: True when drawdown and historical translated NAV are complete. FA: وقتی Drawdown و NAV تاریخی ترجمه‌شده کامل باشند true است.</param>
/// <param name="IsCalculable">EN: True when monetary drawdown can be calculated. FA: وقتی افت مبلغی قابل محاسبه باشد true است.</param>
/// <param name="NetAssetValueBase">EN: Historical NAV at the point in base currency. FA: NAV تاریخی نقطه در ارز پایه.</param>
/// <param name="WealthIndex">EN: Cash-flow-neutral TWR wealth index. FA: شاخص ثروت TWR خنثی نسبت به جریان سرمایه.</param>
/// <param name="RunningPeakWealthIndex">EN: Running peak wealth index. FA: قله جاری شاخص ثروت.</param>
/// <param name="RunningPeakTimestamp">EN: Timestamp of the running peak. FA: زمان قله جاری.</param>
/// <param name="Drawdown">EN: Relative drawdown, zero at peak and negative below it. FA: افت نسبی، در قله صفر و پایین‌تر از آن منفی.</param>
/// <param name="EquivalentPeakNetAssetValueBase">EN: Peak-equivalent NAV at the point's current capital scale. FA: NAV معادل قله با مقیاس سرمایه همان نقطه.</param>
/// <param name="DrawdownAmountBase">EN: Non-negative monetary drawdown amount in base currency. FA: مبلغ غیرمنفی افت سرمایه در ارز پایه.</param>
public sealed record PortfolioMonetaryDrawdownPointResponse(
    DateTimeOffset Timestamp,
    bool IsComplete,
    bool IsCalculable,
    decimal? NetAssetValueBase,
    decimal? WealthIndex,
    decimal? RunningPeakWealthIndex,
    DateTimeOffset? RunningPeakTimestamp,
    decimal? Drawdown,
    decimal? EquivalentPeakNetAssetValueBase,
    decimal? DrawdownAmountBase);

/// <summary>
/// EN: Historical cash-flow-neutral drawdown with base-currency monetary amounts.
/// FA: افت سرمایه تاریخی خنثی نسبت به جریان سرمایه همراه با مبالغ ارز پایه.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved sampling interval. FA: فاصله نمونه‌برداری resolve‌شده.</param>
/// <param name="IsComplete">EN: True when source drawdown and every historical NAV point are complete. FA: وقتی Drawdown مبنا و NAV تاریخی همه نقاط کامل باشند true است.</param>
/// <param name="CurrentDrawdown">EN: Relative drawdown at To. FA: افت نسبی در To.</param>
/// <param name="CurrentDrawdownAmountBase">EN: Monetary drawdown at To. FA: مبلغ افت سرمایه در To.</param>
/// <param name="MaximumDrawdown">EN: Most negative relative drawdown. FA: منفی‌ترین افت نسبی.</param>
/// <param name="MaximumDrawdownAmountBase">EN: Monetary amount at the maximum-relative-drawdown trough. FA: مبلغ افت در کف مربوط به بیشینه افت نسبی.</param>
/// <param name="MaximumDrawdownPeakTimestamp">EN: Peak timestamp preceding maximum drawdown. FA: زمان قله پیش از بیشینه افت.</param>
/// <param name="MaximumDrawdownTroughTimestamp">EN: Trough timestamp of maximum drawdown. FA: زمان کف بیشینه افت.</param>
/// <param name="Points">EN: Ordered monetary drawdown points. FA: نقاط مرتب افت مبلغی.</param>
public sealed record GetPortfolioMonetaryDrawdownResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    bool IsComplete,
    decimal? CurrentDrawdown,
    decimal? CurrentDrawdownAmountBase,
    decimal? MaximumDrawdown,
    decimal? MaximumDrawdownAmountBase,
    DateTimeOffset? MaximumDrawdownPeakTimestamp,
    DateTimeOffset? MaximumDrawdownTroughTimestamp,
    IReadOnlyCollection<PortfolioMonetaryDrawdownPointResponse> Points);
