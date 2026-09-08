// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetInvestorById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Investors.Queries.GetInvestorById;

/// <summary>EN: Query for an investor by ID. FA: پرس‌وجوی دریافت سرمایه‌گذار با شناسه.</summary>
public sealed record GetInvestorByIdQuery(string Id)
    : IRequest<Result<GetInvestorByIdResponse>>;
