// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown;

/// <summary>
/// EN: Requests cash-flow-neutral drawdown translated into base-currency monetary amounts.
/// FA: افت سرمایه خنثی نسبت به جریان سرمایه را به صورت مبلغی در ارز پایه درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Sampling interval: Daily or Weekly. FA: فاصله نمونه‌برداری: Daily یا Weekly.</param>
public sealed record GetPortfolioMonetaryDrawdownQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval)
    : IRequest<Result<GetPortfolioMonetaryDrawdownResponse>>;
