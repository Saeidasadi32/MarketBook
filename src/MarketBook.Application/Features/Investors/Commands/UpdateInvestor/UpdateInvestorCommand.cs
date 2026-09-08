// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.UpdateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.UpdateInvestor;

/// <summary>EN: Command for updating an investor. FA: فرمان به‌روزرسانی سرمایه‌گذار.</summary>
public sealed record UpdateInvestorCommand(string Id, string FullName)
    : IRequest<Result<InvestorId>>;

/// <summary>EN: API request for updating an investor. FA: درخواست API برای به‌روزرسانی سرمایه‌گذار.</summary>
public sealed record UpdateInvestorRequest(string FullName);
