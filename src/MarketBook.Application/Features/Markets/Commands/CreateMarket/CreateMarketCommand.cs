// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.CreateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.CreateMarket;

/// <summary>
/// EN: Represents a request to create a new market.
/// FA: درخواست ایجاد یک بازار جدید را نمایش می‌دهد.
/// </summary>
/// <param name="Code">
/// EN: Unique market business code.
/// FA: کد تجاری یکتای بازار.
/// </param>
/// <param name="Name">
/// EN: Market display name.
/// FA: نام نمایشی بازار.
/// </param>
/// <param name="ExchangeId">
/// EN: Optional exchange identifier.
/// FA: شناسه اختیاری بورس.
/// </param>
public sealed record CreateMarketCommand(
    string Code,
    string Name,
    string? ExchangeId) : IRequest<Result<MarketId>>;
