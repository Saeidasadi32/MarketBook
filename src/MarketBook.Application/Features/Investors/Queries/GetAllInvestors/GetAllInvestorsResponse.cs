// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetAllInvestors
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Investors.Queries.GetAllInvestors;

/// <summary>EN: Investor list item. FA: آیتم فهرست سرمایه‌گذار.</summary>
public sealed record InvestorListItemResponse(
    string Id,
    string FullName,
    DateTimeOffset CreatedOn,
    bool IsActive);

/// <summary>EN: Paged investor response. FA: پاسخ صفحه‌بندی‌شده سرمایه‌گذاران.</summary>
public sealed record GetAllInvestorsResponse(
    IReadOnlyList<InvestorListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
