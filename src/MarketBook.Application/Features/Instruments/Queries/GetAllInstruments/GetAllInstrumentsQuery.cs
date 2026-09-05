// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetAllInstruments
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Queries.GetAllInstruments;

/// <summary>
/// EN: Represents a paged instrument query.
/// FA: پرس‌وجوی صفحه‌بندی‌شده ابزارهای مالی را نشان می‌دهد.
/// </summary>
public sealed record GetAllInstrumentsQuery(
    int Page = 1,
    int PageSize = 20) : IRequest<Result<GetAllInstrumentsResponse>>;
