// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.DeactivateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Commands.DeactivateCurrency;
/// <summary>EN: Represents a command to deactivate a currency. FA: فرمان غیرفعال‌سازی ارز را نشان می‌دهد.</summary>
public sealed record DeactivateCurrencyCommand(string Id) : IRequest<Result<CurrencyId>>;
