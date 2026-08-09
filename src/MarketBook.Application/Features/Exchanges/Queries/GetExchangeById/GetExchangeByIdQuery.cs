// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Queries.GetExchangeById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Exchanges.Queries.GetExchangeById;

/// <summary>
/// EN: Represents a query for retrieving an exchange by identifier.
/// FA: پرس‌وجوی دریافت بورس بر اساس شناسه.
/// </summary>
public sealed record GetExchangeByIdQuery(
    string Id)
    : IQuery<Result<ExchangeResponse>>;
