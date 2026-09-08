// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.MarketData.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick;

/// <summary>
/// EN: Command for creating an intraday price tick.
/// FA: فرمان ایجاد Tick قیمت درون‌روزی.
/// </summary>
public sealed record CreateIntradayPriceTickCommand(
    string ListingId,
    DateOnly TradingDate,
    DateTimeOffset OccurredAt,
    long SequenceNumber,
    decimal Price,
    long Volume)
    : IRequest<Result<IntradayPriceTickId>>;
