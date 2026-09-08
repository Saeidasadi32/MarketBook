// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Commands.CreateFxRate
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.FxRates.Commands.CreateFxRate;

/// <summary>
/// EN: HTTP request for creating a dated FX rate.
/// FA: درخواست HTTP برای ایجاد نرخ تاریخ‌دار ارز.
/// </summary>
public sealed record CreateFxRateRequest(
    string BaseCurrencyId,
    string QuoteCurrencyId,
    DateOnly RateDate,
    decimal Rate);
