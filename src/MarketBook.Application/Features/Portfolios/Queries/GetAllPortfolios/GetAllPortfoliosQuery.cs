// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;

/// <summary>
/// EN: Requests paged portfolios with an optional investor filter.
/// FA: درخواست پرتفوی‌های صفحه‌بندی‌شده با فیلتر اختیاری سرمایه‌گذار را نمایش می‌دهد.
/// </summary>
public sealed record GetAllPortfoliosQuery(
    int Page,
    int PageSize,
    string? InvestorId) : IRequest<Result<GetAllPortfoliosResponse>>;
