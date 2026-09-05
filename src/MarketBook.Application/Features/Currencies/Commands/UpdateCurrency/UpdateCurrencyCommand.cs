// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.UpdateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Commands.UpdateCurrency;
/// <summary>EN: Represents a command to update a currency. FA: فرمان به‌روزرسانی ارز را نشان می‌دهد.</summary>
public sealed record UpdateCurrencyCommand(string Id, string Code, string Name, int DecimalPlaces) : IRequest<Result<CurrencyId>>;
