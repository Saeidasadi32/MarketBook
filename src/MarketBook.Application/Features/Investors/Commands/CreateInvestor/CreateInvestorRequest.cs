// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.CreateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.Investors.Commands.CreateInvestor;

/// <summary>EN: API request for creating an investor. FA: درخواست API برای ایجاد سرمایه‌گذار.</summary>
public sealed record CreateInvestorRequest(string FullName);
