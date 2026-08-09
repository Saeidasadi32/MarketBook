// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Queries.GetExchanges
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Exchanges.Queries.GetExchanges;

/// <summary>
/// EN: Represents a query for retrieving exchanges with pagination.
/// FA: پرس‌وجوی دریافت بورس‌های صفحه‌بندی‌شده.
/// </summary>
public sealed record GetExchangesQuery(
    int Page = PageRequest.DefaultPage,
    int PageSize = PageRequest.DefaultPageSize)
    : IQuery<Result<PagedResult<ExchangeResponse>>>;
