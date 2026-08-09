// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.CreateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.ValueObjects;

namespace MarketBook.Application.Features.Exchanges.Commands.CreateExchange;

/// <summary>
/// EN: Represents a command for creating an exchange.
/// FA: فرمان ایجاد یک بورس را نمایش می‌دهد.
/// </summary>
public sealed record CreateExchangeCommand(
    string CountryId,
    string Code,
    string Name)
    : ICommand<Result<ExchangeId>>;
