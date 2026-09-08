// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate;

/// <summary>
/// EN: Requests the latest exact-direction FX quote for a currency pair.
/// FA: آخرین نرخ ارز با جهت دقیق برای یک جفت‌ارز را درخواست می‌کند.
/// </summary>
public sealed record GetLatestFxRateQuery(
    string BaseCurrencyId,
    string QuoteCurrencyId)
    : IRequest<Result<GetLatestFxRateResponse>>;
