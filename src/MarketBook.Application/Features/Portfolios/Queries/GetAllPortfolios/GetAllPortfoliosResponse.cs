// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;

/// <summary>
/// EN: Represents a portfolio list item.
/// FA: یک آیتم از فهرست پرتفوی‌ها را نمایش می‌دهد.
/// </summary>
public sealed record PortfolioListItemResponse(
    string Id,
    string InvestorId,
    string Name,
    DateTimeOffset CreatedOn,
    bool IsActive);

/// <summary>
/// EN: Represents a paged portfolio response.
/// FA: پاسخ صفحه‌بندی‌شده پرتفوی‌ها را نمایش می‌دهد.
/// </summary>
public sealed record GetAllPortfoliosResponse(
    IReadOnlyCollection<PortfolioListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
