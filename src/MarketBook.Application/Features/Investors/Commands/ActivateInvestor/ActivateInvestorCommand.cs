// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.ActivateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.ActivateInvestor;

/// <summary>EN: Command to activate an investor. FA: فرمان فعال‌سازی سرمایه‌گذار.</summary>
public sealed record ActivateInvestorCommand(string Id)
    : IRequest<Result<InvestorId>>;
