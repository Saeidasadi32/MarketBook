// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;

/// <summary>
/// EN: Requests a portfolio by identifier.
/// FA: درخواست دریافت پرتفوی بر اساس شناسه را نمایش می‌دهد.
/// </summary>
public sealed record GetPortfolioByIdQuery(
    string Id) : IRequest<Result<GetPortfolioByIdResponse>>;
