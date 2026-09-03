// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.DeactivateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Exchanges.Commands.DeactivateExchange;

/// <summary>
/// EN: Represents a request to deactivate an existing exchange.
/// FA: درخواست غیرفعال‌سازی یک بورس موجود را نمایش می‌دهد.
/// </summary>
/// <param name="Id">
/// EN: Exchange identifier.
/// FA: شناسه بورس.
/// </param>
public sealed record DeactivateExchangeCommand(
    string Id) : IRequest<Result<ExchangeId>>;
