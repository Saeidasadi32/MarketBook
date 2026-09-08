// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;

/// <summary>
/// EN: Represents a portfolio API response.
/// FA: پاسخ API مربوط به پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record GetPortfolioByIdResponse(
    string Id,
    string InvestorId,
    string Name,
    DateTimeOffset CreatedOn,
    bool IsActive);
