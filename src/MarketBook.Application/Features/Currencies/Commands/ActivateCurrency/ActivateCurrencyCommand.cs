// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.ActivateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Commands.ActivateCurrency;
/// <summary>EN: Represents a command to activate a currency. FA: فرمان فعال‌سازی ارز را نشان می‌دهد.</summary>
public sealed record ActivateCurrencyCommand(string Id) : IRequest<Result<CurrencyId>>;
