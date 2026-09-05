// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.CreateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Currencies.Commands.CreateCurrency;

/// <summary>
/// EN: Represents a command to create a currency.
/// FA: فرمان ایجاد یک ارز را نشان می‌دهد.
/// </summary>
public sealed record CreateCurrencyCommand(string Code, string Name, int DecimalPlaces) : IRequest<Result<CurrencyId>>;
