// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.TradingCalendars.Queries.GetAllTradingCalendars;

/// <summary>
/// EN: Represents a paged trading-calendar query.
/// FA: پرس‌وجوی صفحه‌بندی‌شده تقویم‌های معاملاتی را نشان می‌دهد.
/// </summary>
public sealed record GetAllTradingCalendarsQuery(
    int Page = 1,
    int PageSize = 20) : IRequest<Result<GetAllTradingCalendarsResponse>>;
