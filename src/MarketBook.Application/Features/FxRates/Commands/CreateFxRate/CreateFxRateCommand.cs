// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Commands.CreateFxRate
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.FxRates.Commands.CreateFxRate;

/// <summary>
/// EN: Creates a dated FX-rate record.
/// FA: یک رکورد تاریخ‌دار نرخ ارز ایجاد می‌کند.
/// </summary>
public sealed record CreateFxRateCommand(
    string BaseCurrencyId,
    string QuoteCurrencyId,
    DateOnly RateDate,
    decimal Rate)
    : IRequest<Result<FxRateId>>;
