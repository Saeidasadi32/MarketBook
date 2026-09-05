// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.UpdateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
namespace MarketBook.Application.Features.Currencies.Commands.UpdateCurrency;
/// <summary>EN: Represents a request to update a currency. FA: درخواست به‌روزرسانی ارز را نشان می‌دهد.</summary>
public sealed record UpdateCurrencyRequest(string Code, string Name, int DecimalPlaces);
