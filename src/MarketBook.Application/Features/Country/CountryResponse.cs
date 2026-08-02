// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Country
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Country;

/// <summary>
/// EN: Represents country information returned to clients.
/// FA: اطلاعات کشور که به کلاینت بازگردانده می‌شود.
/// </summary>
public sealed record CountryResponse(
    string Id,
    string Code,
    string Name,
    bool IsActive);