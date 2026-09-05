// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetInstrumentById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Queries.GetInstrumentById;

/// <summary>
/// EN: Represents a query to retrieve an instrument by identifier.
/// FA: پرس‌وجوی دریافت ابزار مالی بر اساس شناسه را نشان می‌دهد.
/// </summary>
public sealed record GetInstrumentByIdQuery(
    string Id) : IRequest<Result<GetInstrumentByIdResponse>>;
