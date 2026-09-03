// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.CreateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Markets.Commands.CreateMarket;

/// <summary>
/// EN: Represents the HTTP request to create a market.
/// FA: درخواست HTTP برای ایجاد بازار را نمایش می‌دهد.
/// </summary>
/// <param name="Code">
/// EN: Unique market business code.
/// FA: کد تجاری یکتای بازار.
/// </param>
/// <param name="Name">
/// EN: Market display name.
/// FA: نام نمایشی بازار.
/// </param>
/// <param name="ExchangeId">
/// EN: Optional exchange identifier.
/// FA: شناسه اختیاری بورس.
/// </param>
public sealed record CreateMarketRequest(
    string Code,
    string Name,
    string? ExchangeId);
