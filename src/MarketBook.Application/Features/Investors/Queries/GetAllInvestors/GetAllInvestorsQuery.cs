// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetAllInvestors
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Investors.Queries.GetAllInvestors;

/// <summary>EN: Query for paged investors. FA: پرس‌وجوی سرمایه‌گذاران صفحه‌بندی‌شده.</summary>
public sealed record GetAllInvestorsQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<GetAllInvestorsResponse>>;
