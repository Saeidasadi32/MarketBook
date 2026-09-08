// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate;

/// <summary>
/// EN: Latest exact-direction FX quote response.
/// FA: پاسخ آخرین نرخ ارز با جهت دقیق.
/// </summary>
public sealed record GetLatestFxRateResponse(
    string Id,
    string BaseCurrencyId,
    string QuoteCurrencyId,
    DateOnly RateDate,
    decimal Rate,
    DateTimeOffset CreatedOn);
