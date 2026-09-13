// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicy;

/// <summary>EN: Creates the first active persisted risk policy. FA: اولین Policy ریسک فعال را ایجاد می‌کند.</summary>
public sealed record CreatePortfolioRiskPolicyCommand(
    string PortfolioId,
    PortfolioRiskPolicyLimitsRequest Limits)
    : IRequest<Result<PortfolioRiskPolicyResponse>>;
