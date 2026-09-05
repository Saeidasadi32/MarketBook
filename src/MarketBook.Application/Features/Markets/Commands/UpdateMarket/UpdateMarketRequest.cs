// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.CreateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Markets.Commands.UpdateMarket;


/// <summary>
/// EN: Represents the HTTP request body used to update a market.
/// FA: بدنه درخواست HTTP برای به‌روزرسانی بازار را نمایش می‌دهد.
/// </summary>
public sealed record UpdateMarketRequest(
    string Code,
    string Name,
    string? ExchangeId);
