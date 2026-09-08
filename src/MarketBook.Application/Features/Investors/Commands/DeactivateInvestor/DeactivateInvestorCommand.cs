// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.DeactivateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.DeactivateInvestor;

/// <summary>EN: Command to deactivate an investor. FA: فرمان غیرفعال‌سازی سرمایه‌گذار.</summary>
public sealed record DeactivateInvestorCommand(string Id)
    : IRequest<Result<InvestorId>>;
