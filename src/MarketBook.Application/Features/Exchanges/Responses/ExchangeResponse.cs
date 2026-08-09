// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Responses
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Exchanges.Responses;

/// <summary>
/// EN: Represents an exchange response.
/// FA: پاسخ مربوط به بورس را نمایش می‌دهد.
/// </summary>
public sealed record ExchangeResponse(
    string Id,
    string CountryId,
    string Code,
    string Name,
    DateTimeOffset CreatedOn,
    bool IsActive);
