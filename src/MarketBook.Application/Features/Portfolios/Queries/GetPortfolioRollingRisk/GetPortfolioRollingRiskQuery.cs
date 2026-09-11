// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk;

/// <summary>
/// EN: Requests rolling portfolio risk analytics.
/// FA: تحلیل ریسک Rolling پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="WindowPeriods">EN: Number of periodic-return observations in each rolling window. FA: تعداد مشاهدات بازده دوره‌ای در هر پنجره Rolling.</param>
/// <param name="RiskFreeRateAnnual">EN: Annual arithmetic risk-free rate. FA: نرخ بدون‌ریسک حسابی سالانه.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual arithmetic minimum acceptable return. FA: حداقل بازده قابل‌قبول حسابی سالانه.</param>
public sealed record GetPortfolioRollingRiskQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    int WindowPeriods = 30,
    decimal RiskFreeRateAnnual = 0m,
    decimal MinimumAcceptableReturnAnnual = 0m)
    : IRequest<Result<GetPortfolioRollingRiskResponse>>;
