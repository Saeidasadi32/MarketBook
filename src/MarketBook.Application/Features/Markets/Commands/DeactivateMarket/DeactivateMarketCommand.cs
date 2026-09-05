// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.DeactivateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.DeactivateMarket;

/// <summary>
/// EN: Represents a request to deactivate an existing market.
/// FA: درخواست غیرفعال‌سازی یک بازار موجود را نمایش می‌دهد.
/// </summary>
public sealed record DeactivateMarketCommand(
    string Id) : IRequest<Result<MarketId>>;
