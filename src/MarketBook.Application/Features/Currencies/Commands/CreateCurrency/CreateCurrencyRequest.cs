// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.CreateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Currencies.Commands.CreateCurrency;

/// <summary>
/// EN: Represents a request to create a currency.
/// FA: درخواست ایجاد یک ارز را نشان می‌دهد.
/// </summary>
public sealed record CreateCurrencyRequest(string Code, string Name, int DecimalPlaces);
