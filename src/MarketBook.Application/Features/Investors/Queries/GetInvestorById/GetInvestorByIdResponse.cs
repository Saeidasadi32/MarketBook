// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetInvestorById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Investors.Queries.GetInvestorById;

/// <summary>EN: Investor detail response. FA: پاسخ جزئیات سرمایه‌گذار.</summary>
public sealed record GetInvestorByIdResponse(
    string Id,
    string FullName,
    DateTimeOffset CreatedOn,
    bool IsActive);
