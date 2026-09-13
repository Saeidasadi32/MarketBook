// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot;

/// <summary>
/// EN: Requests the latest compact rolling risk dashboard snapshot.
/// FA: آخرین snapshot فشرده داشبورد ریسک Rolling را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="WindowPeriods">EN: Full rolling-window size in periodic returns. FA: اندازه پنجره کامل Rolling بر حسب بازده‌های دوره‌ای.</param>
/// <param name="ConfidenceLevel">EN: Historical VaR/CVaR confidence level. FA: سطح اطمینان VaR/CVaR تاریخی.</param>
/// <param name="RiskFreeRateAnnual">EN: Annual arithmetic risk-free rate. FA: نرخ بدون‌ریسک حسابی سالانه.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual arithmetic minimum acceptable return. FA: حداقل بازده قابل‌قبول حسابی سالانه.</param>
public sealed record GetPortfolioRollingRiskSummarySnapshotQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    int WindowPeriods = 30,
    decimal ConfidenceLevel = 0.95m,
    decimal RiskFreeRateAnnual = 0m,
    decimal MinimumAcceptableReturnAnnual = 0m)
    : IRequest<Result<GetPortfolioRollingRiskSummarySnapshotResponse>>;
