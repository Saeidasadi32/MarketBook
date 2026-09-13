// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskSummary;

/// <summary>
/// EN: Requests a compact portfolio risk dashboard composed from existing projections.
/// FA: داشبورد فشرده ریسک پرتفوی را از projectionهای موجود درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
/// <param name="ConfidenceLevel">EN: Historical VaR/CVaR confidence level. FA: سطح اطمینان VaR/CVaR تاریخی.</param>
/// <param name="RiskFreeRateAnnual">EN: Annual arithmetic risk-free rate. FA: نرخ بدون‌ریسک حسابی سالانه.</param>
/// <param name="MinimumAcceptableReturnAnnual">EN: Annual arithmetic minimum acceptable return. FA: حداقل بازده قابل‌قبول حسابی سالانه.</param>
public sealed record GetPortfolioRiskSummaryQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    decimal ConfidenceLevel = 0.95m,
    decimal RiskFreeRateAnnual = 0m,
    decimal MinimumAcceptableReturnAnnual = 0m)
    : IRequest<Result<GetPortfolioRiskSummaryResponse>>;
