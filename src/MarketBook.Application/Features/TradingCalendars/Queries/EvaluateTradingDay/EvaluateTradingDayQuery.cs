// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.EvaluateTradingDay;

/// <summary>
/// EN: Evaluates one date against a trading calendar.
/// FA: یک تاریخ را در برابر تقویم معاملاتی ارزیابی می‌کند.
/// </summary>
public sealed record EvaluateTradingDayQuery(
    string Id,
    DateOnly Date) : IRequest<Result<EvaluateTradingDayResponse>>;
