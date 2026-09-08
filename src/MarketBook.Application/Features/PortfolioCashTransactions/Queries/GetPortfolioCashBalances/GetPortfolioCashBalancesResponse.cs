// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


namespace MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances;

/// <summary>EN: Projected balance for one currency. FA: موجودی محاسبه‌شده برای یک ارز.</summary>
public sealed record PortfolioCashBalanceItemResponse(
    string CurrencyId,
    decimal Balance);

/// <summary>EN: Portfolio multi-currency cash-balance projection. FA: تصویر موجودی نقدی چندارزی پرتفوی.</summary>
public sealed record GetPortfolioCashBalancesResponse(
    IReadOnlyList<PortfolioCashBalanceItemResponse> Items);
