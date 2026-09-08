// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.CreateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.CreateInvestor;

/// <summary>EN: Command for creating an investor. FA: فرمان ایجاد سرمایه‌گذار.</summary>
public sealed record CreateInvestorCommand(string FullName)
    : IRequest<Result<InvestorId>>;
