// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.UpdateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Exchanges.Commands.UpdateExchange;
/// <summary>
/// EN: Represents a request to update an existing exchange.
/// FA: درخواست به‌روزرسانی یک بورس موجود را نمایش می‌دهد.
/// </summary>
/// <param name="Code">
/// EN: New exchange business code.
/// FA: کد تجاری جدید بورس.
/// </param>
/// <param name="Name">
/// EN: New exchange display name.
/// FA: نام نمایشی جدید بورس.
/// </param>
/// <param name="CountryId">
/// EN: Optional country identifier.
/// FA: شناسه اختیاری کشور.
/// </param>
public sealed record UpdateExchangeRequest(
    string Code,
    string Name,
    string? CountryId);
